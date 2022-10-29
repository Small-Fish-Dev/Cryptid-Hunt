using SpookyJam2022.States;

namespace SpookyJam2022;

public partial class NextBotPlayer : BasePlayer
{
	private readonly ClothingContainer _clothing = new();
	private readonly IEnumerable<NextbotPlayerSpawn> _spawnPoints;
	
	private DamageInfo _lastDamage;

	public NextBotPlayer()
	{
		_spawnPoints = Entity.All.OfType<NextbotPlayerSpawn>();
	}

	public override void Spawn()
	{
		base.Spawn();
		
		Controller ??= new WalkController();
		
		SetModel( "models/citizen/citizen.vmdl" );

		EnableShadowCasting = false;
		EnableDrawing = false;

		_clothing.LoadFromClient( Game.PlayerClient );
		_clothing.DressEntity( this );
	}

	public override void Respawn()
	{
		LifeState = LifeState.Alive;
		Health = 1f;
		
		var randomSpawnPoint = _spawnPoints.MinBy(x => Guid.NewGuid());
		Position = randomSpawnPoint.Position;
		Rotation = randomSpawnPoint.Rotation;
		
		Event.Run( "nextbot.player.respawn" );
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		EyeRotation = Input.Rotation;
	}

	[ConCmd.Admin( "debug_commit_die" )]
	public static void CommitDie()
	{
		if ( Game.PlayerClient.Pawn is not NextBotPlayer player )
			throw new Exception("Pawn is not NextBot player");

		player.TakeDamage( DamageInfo.Generic( 50f ) );
	}
	
	public override void TakeDamage( DamageInfo info )
	{
		_lastDamage = info;

		base.TakeDamage( info );
	}

	public override void OnKilled()
	{
		LifeState = LifeState.Dead;

		BecomeRagdollOnClient( Velocity, _lastDamage.Flags, _lastDamage.Position, _lastDamage.Force,
			_lastDamage.BoneIndex );

		Event.Run( "nextbot.player.dead" );

		GameTask.RunInThreadAsync( async () =>
		{
			await GameTask.DelaySeconds( 1.0f );
			if ( IsValid )
				Respawn();
		} );
	}

	ModelEntity corpse;

	[ClientRpc]
	private void BecomeRagdollOnClient( Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone )
	{
		corpse = new ModelEntity();
		corpse.Tags.Add( "ragdoll", "solid", "debris" );
		corpse.Position = Position;
		corpse.Rotation = Rotation;
		corpse.Scale = Scale;
		corpse.UsePhysicsCollision = true;
		corpse.EnableAllCollisions = true;
		corpse.SetModel( GetModelName() );
		corpse.CopyBonesFrom( this );
		corpse.CopyBodyGroups( this );
		corpse.CopyMaterialGroup( this );
		corpse.CopyMaterialOverrides( this );
		corpse.TakeDecalsFrom( this );
		corpse.EnableAllCollisions = true;
		corpse.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		corpse.RenderColor = RenderColor;
		corpse.PhysicsGroup.Velocity = velocity;
		corpse.PhysicsEnabled = true;

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			var model = e.GetModelName();

			var clothing = new ModelEntity();
			clothing.SetModel( model );
			clothing.SetParent( corpse, true );
			clothing.RenderColor = e.RenderColor;
			clothing.CopyBodyGroups( e );
			clothing.CopyMaterialGroup( e );
		}

		if ( damageFlags.HasFlag( DamageFlags.Bullet ) ||
		     damageFlags.HasFlag( DamageFlags.PhysicsImpact ) )
		{
			PhysicsBody body = bone > 0 ? corpse.GetBonePhysicsBody( bone ) : null;

			if ( body != null )
			{
				body.ApplyImpulseAt( forcePos, force * body.Mass );
			}
			else
			{
				corpse.PhysicsGroup.ApplyImpulse( force );
			}
		}

		if ( damageFlags.HasFlag( DamageFlags.Blast ) )
		{
			if ( corpse.PhysicsGroup != null )
			{
				corpse.PhysicsGroup.AddVelocity( (Position - (forcePos + Vector3.Down * 100.0f)).Normal * (force.Length * 0.2f) );
				var angularDir = (Rotation.FromYaw( 90 ) * force.WithZ( 0 ).Normal).Normal;
				corpse.PhysicsGroup.AddAngularVelocity( angularDir * (force.Length * 0.02f) );
			}
		}

		corpse.DeleteAsync( 10.0f );
	}

	private FearEntry fearEntry;

	public override void BuildInput( InputBuilder input )
	{
		if ( fearEntry is not null && fearEntry.IsValid )
		{
			if ( input.Pressed( InputButton.PrimaryAttack ) )
				fearEntry.Focus();
		} else {
			input.ViewAngles += input.AnalogLook;
			input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
			input.InputDirection = input.AnalogMove;
		}
	}

	WorldPanel screen;
	Panel view;
	SceneCamera camera;
	Extensions.SceneRender render;
	ModelEntity computer;
	Texture lastFrame;
	SceneLight light;

	[Event.Frame]
	private void onFrame()
	{
		if ( Local.Pawn is NextBotPlayer || lastFrame != null ) return;

		lastFrame = render.Texture;
		view.Style.BackgroundImage = lastFrame;
		view.SetClass( "paused", true );
		view.AddChild<Label>( "pause" ).Text = "PAUSED";

		render.Delete();
		camera.World = null;
		camera = null;

		light.Delete();
	}

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		if ( computer == null )
		{
			foreach ( var ent in Entity.All ) // find by name doesn't work x 
			{
				if ( ent is not ModelEntity mdl ) continue;
				if ( !ent.Name.Contains( "computer" ) ) continue;

				computer = mdl;
				break;
			}

			return;
		}

		if ( screen == null )
		{
			screen = new();
			screen.StyleSheet = HUD.Instance.StyleSheet;
			screen.WorldScale = 0.3f;

			var panelSize = new Vector2( 540, 320 ) * (1f / screen.WorldScale);
			screen.PanelBounds = new Rect( -panelSize / 2f, panelSize );

			camera = new SceneCamera( "nextBotCamera" )
			{
				FieldOfView = 70f,
				World = Map.Scene,
				Position = Position + Vector3.Up * 64f,
				Rotation = EyeRotation
			};

			render = camera.Render( new Vector2( 459, 272 ), false, 20 );

			view = screen.AddChild<Panel>( "Screen" );
			view.Style.BackgroundImage = render.Texture;

			view.AddChild<Label>( "watermark" ).Text = "MADE BY MONKEY BAR";

			var fearContainer = screen.AddChild<Panel>( "fear" );
			fearContainer.Add.Label( "Welcome to the NextBot Personalised!\nWhat or who do you fear the most?.." );
			fearContainer.Add.Label( "(press Primary Attack button to focus)", "techtip" );
			fearEntry = fearContainer.AddChild<FearEntry>();
			fearEntry.Focus();
		}

		if ( light == null )
		{
			light = new( Map.Scene )
			{
				LightColor = Color.White,
				Radius = 100f
			};
		} 
		else
		{
			light.Position = camera.Position + Vector3.Up * 10f;
			light.Rotation = Rotation.From( 89, 0, 0 );
		}

		camera.Position = (corpse?.IsValid() ?? false) && LifeState == LifeState.Dead
			? Vector3.Lerp( camera.Position, corpse.Position + Vector3.Up * 80f, 10f * Time.Delta )
			: Position + Vector3.Up * 64f;
		camera.Rotation = (corpse?.IsValid() ?? false) && LifeState == LifeState.Dead
			? Rotation.Lerp( camera.Rotation, Rotation.From( 89, 0, 0 ), 10f * Time.Delta )
			: EyeRotation;

		var attachmentPos = computer?.GetAttachment( "screen" )?.Position ?? Vector3.Zero;
		setup.Viewer = this;
		setup.Position = attachmentPos
			+ Vector3.Right * 25f;
		setup.Rotation = Rotation.LookAt( attachmentPos - setup.Position );
		setup.FieldOfView = 65f;

		screen.Rotation = setup.Rotation.Inverse;
		screen.Position = attachmentPos - Vector3.Right * 0.01f;
	}
}
