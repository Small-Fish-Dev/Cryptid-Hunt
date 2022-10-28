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
		
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		
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
			await GameTask.DelaySeconds( 2.0f );
			Respawn();
		} );
	}
	
	[ClientRpc]
	private void BecomeRagdollOnClient( Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone )
	{
		var ent = new ModelEntity();
		ent.Tags.Add( "ragdoll", "solid", "debris" );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Scale = Scale;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.CopyBodyGroups( this );
		ent.CopyMaterialGroup( this );
		ent.CopyMaterialOverrides( this );
		ent.TakeDecalsFrom( this );
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ent.RenderColor = RenderColor;
		ent.PhysicsGroup.Velocity = velocity;
		ent.PhysicsEnabled = true;

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			var model = e.GetModelName();

			var clothing = new ModelEntity();
			clothing.SetModel( model );
			clothing.SetParent( ent, true );
			clothing.RenderColor = e.RenderColor;
			clothing.CopyBodyGroups( e );
			clothing.CopyMaterialGroup( e );
		}

		if ( damageFlags.HasFlag( DamageFlags.Bullet ) ||
		     damageFlags.HasFlag( DamageFlags.PhysicsImpact ) )
		{
			PhysicsBody body = bone > 0 ? ent.GetBonePhysicsBody( bone ) : null;

			if ( body != null )
			{
				body.ApplyImpulseAt( forcePos, force * body.Mass );
			}
			else
			{
				ent.PhysicsGroup.ApplyImpulse( force );
			}
		}

		if ( damageFlags.HasFlag( DamageFlags.Blast ) )
		{
			if ( ent.PhysicsGroup != null )
			{
				ent.PhysicsGroup.AddVelocity( (Position - (forcePos + Vector3.Down * 100.0f)).Normal * (force.Length * 0.2f) );
				var angularDir = (Rotation.FromYaw( 90 ) * force.WithZ( 0 ).Normal).Normal;
				ent.PhysicsGroup.AddAngularVelocity( angularDir * (force.Length * 0.02f) );
			}
		}

		ent.DeleteAsync( 10.0f );
	}

	WorldPanel screen;
	Panel view;
	SceneCamera camera;
	Extensions.SceneRender render;
	ModelEntity computer;

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		if ( computer == null )
		{
			/*computer = Entity.All
				.Where( ent => (ent as ModelEntity)?.GetModelName() == "models/bedroom/computer/computer.vmdl" ) 
				as ModelEntity;*/
			// i don't really know please make the computer possible to get from code!!!

			return;
		}

		if ( screen == null )
		{
			screen = new();
			screen.StyleSheet = HUD.Instance.StyleSheet;
			screen.WorldScale = 0.3f;

			var size = new Vector2( 480, 360 ) * (1f / screen.WorldScale);
			screen.PanelBounds = new Rect( -size / 2f, size );

			view = screen.AddChild<Panel>( "Screen" );

			camera = new SceneCamera( "nextBotCamera" )
			{

				FieldOfView = 60f,
				World = Map.Scene
			};

			render = camera.Render( new Vector2( 480, 360 ), false, 24 );
		}

		setup.Viewer = this;
		setup.Position = computer?.GetAttachment( "screen" )?.Position ?? Vector3.Zero; // needs some moving
		setup.Rotation = Transform.Zero.Rotation; // actually rotate towards screen from camera pos

		view.Style.BackgroundImage = render.Texture;
	}
}
