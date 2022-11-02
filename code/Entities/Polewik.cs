using Sandbox;
using SpookyJam2022.States;
using System.Numerics;

namespace SpookyJam2022;

public enum PolewikState
{
	Idle,
	Patrolling,
	Stalking,
	Following,
	Attacking,
	Fleeing,
	Pain,
	Yell,
	AttackPersistent,
	Jumpscare
}



[HammerEntity]
[EditorModel( "models/polewik/polewik.vmdl" )]
[Display( Name = "Polewik", GroupName = "Monster", Description = "the monster" )]
public partial class Polewik : AnimatedEntity
{

	public float JumpscareDistance => 120f;
	public float DetectDistance => 1400f;
	public float StalkingDistance => 700f;
	public float AttackDistance => 500f;
	public float GiveUpDistance => 2600f;
	public float GiveUpAfter => 25f;
	public float AttackAfterStalking => 8f;
	public float AttackAfterStalling => 60f;

	TimeSince lastAttack = 0f;

	[Net, Change] PolewikState currentState { get; set; } = PolewikState.Patrolling;
	public PolewikState CurrentState
	{
		get => currentState;
		set
		{

			currentState = value;

			if ( value == PolewikState.Patrolling ) // Yea yea, ugly!!! Don't have time.
			{

				NavigateTo( NearestNode.WorldPosition );
				CurrentPathId = PatrolPath.PathNodes.IndexOf( NearestNode );

			}

			if ( value == PolewikState.Pain )
			{

				lastAttack = 0f;

				PlaySound( "sounds/polewik/pain.sound" );

				GameTask.RunInThreadAsync( async () =>
				{

					SetAnimParameter( "growl", true );

					await GameTask.DelaySeconds( 0.1f );

					if ( !IsValid ) return;
					SetAnimParameter( "growl", false );

					await GameTask.DelaySeconds( 0.9f );

					if ( !IsValid ) return;

					CurrentState = PolewikState.Fleeing;

				} );

			}

			if ( value == PolewikState.Yell )
			{

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 0.5f );

					if ( !IsValid ) return;
					Event.Run( "ScreenShake", 2f, 7f );
					SetAnimParameter( "howl", true );

					await GameTask.DelaySeconds( 3.5f );

					if ( !IsValid ) return;
					Victim = ClosestPlayer;
					CurrentState = PolewikState.AttackPersistent;

				} );

			}

			if ( value == PolewikState.Fleeing )
			{

				lastAttack = 0f;

				NavigateTo( NearestNode.WorldPosition );
				CurrentPathId = PatrolPath.PathNodes.IndexOf( NearestNode );

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( Rand.Float( 15f, 30f ) );

					if ( !IsValid ) return;
					CurrentState = PolewikState.Patrolling;

				} );

			}

			if ( value == PolewikState.Stalking )
			{

				startedStalking = 0f;

			}

			if ( value == PolewikState.Following )
			{

				startedFollowing = 0f;

			}

			if ( value == PolewikState.AttackPersistent )
			{

				Victim = ClosestPlayer;
				startedFollowing = 0f;

			}

			if ( value == PolewikState.Attacking )
			{

				SetAnimParameter( "leap", true );

				PlaySound( "sounds/polewik/jump.sound" );

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 0.05f );

					SetAnimParameter( "leap", false );

					await GameTask.DelaySeconds( 0.9f );

					if ( !IsValid ) return;
					if ( CurrentState == PolewikState.Attacking )
					{

						CurrentState = PolewikState.Following;

					}

				} );

			}

			if ( value == PolewikState.Jumpscare )
			{

				lastAttack = 0f;

				Velocity = 0f;
				WishVelocity = 0f;
				Rotation = Rotation.LookAt( Victim.Position );

				ResetInterpolation();


				Victim.LockInputs = true;
				Victim.OverrideCamera = new ScriptedEventCamera( this, true );
				Victim.ScriptedEvent = true;

				SetAnimParameter( "attack", true );

				var flashlight = Victim.CreateLight( Victim.OverrideCamera );

				PlaySound( "sounds/polewik/jumpscare.sound" );

				Event.Run( "ScreenShake", 1.6f, 5f );
				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 1.2f );

					if ( !IsValid ) return;
					Victim.HP -= 1;

					await GameTask.DelaySeconds( 0.45f );

					if ( !IsValid ) return;
					flashlight.Delete();

					if ( CurrentState == PolewikState.Jumpscare )
					{

						CurrentState = PolewikState.Fleeing;

					}

					Victim.OverrideCamera.Delete();
					Victim.LockInputs = false;
					Victim.OverrideCamera = null;
					Victim.ScriptedEvent = false;

				} );

			}

		}
	}
	public GenericPathEntity PatrolPath => Entity.All.OfType<GenericPathEntity>().Where( x => x.Name.Contains( "Monster" )).FirstOrDefault();
	public string ModelName => "models/polewik/polewik.vmdl";
	public float Gravity => 700f;
	public Dictionary<PolewikState, float> Speeds = new()
	{
		{ PolewikState.Idle, 0f },
		{ PolewikState.Patrolling, 450f },
		{ PolewikState.Stalking, 300f },
		{ PolewikState.Following, 550f },
		{ PolewikState.Attacking, 1800f },
		{ PolewikState.Fleeing, 750f },
		{ PolewikState.Pain, 0f },
		{ PolewikState.Yell, 0f },
		{ PolewikState.AttackPersistent, 550f }, // TODO Change speed depending on if player manages to reach tower
		{ PolewikState.Jumpscare, 0f }

	};
	public float CurrentSpeed => Speeds[CurrentState];
	public NavAgentHull Agent => NavAgentHull.Default;
	public Vector3 TargetPosition { get; set; }
	public bool ReachedTarget { get; set; } = true;
	public int CurrentPathId { get; set; } = 0;
	public NavPath CurrentPath;
	public float PathLength;

	public BBox CollisionBox;

	[Net] public float hp { get; set; } = 100f; // TODO SET TO 100
	public float HP
	{
		get => hp;
		set
		{

			var damage = hp - value;

			hp = value;

			if ( HP <= 0 )
			{

				RagdollModel( this );
				Sound.FromWorld( "sounds/polewik/pain.sound", Position );

				new Action( async () =>
				{

					await GameTask.DelaySeconds( 6f );

					Game.Instance.StartBlackScreen();

					await GameTask.DelaySeconds( 2.5f );

					Game.State = new AfterGameState();


				} ).Invoke();

				Delete();

			}

			if ( damage >= 10 )
			{

				if ( CurrentState != PolewikState.Pain && CurrentState != PolewikState.Fleeing )
				{

					CurrentState = PolewikState.Pain;

				}

			}

		}

	}

	[Net] bool disabled { get; set; } = true;
	public bool Disabled
	{
		get { return disabled; }
		set
		{
			disabled = value;
			stuckOnMovement = 0f;

		}
	}

	public Player Victim;

	public override void Spawn()
	{

		base.Spawn();

		SetModel( ModelName );

		CollisionBox = new BBox( new Vector3( -18f, -18f, 0f ), new Vector3( 18f, 18f, 60f ) );
		EnableHitboxes = true;

		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, CollisionBox.Mins, CollisionBox.Maxs );

		Tags.Add( "Polewik" );
		Tags.Add( "solid" );

	}

	public Player ClosestPlayer => Entity.All.OfType<Player>().OrderBy( x => x.Position.Distance( this.Position ) ).FirstOrDefault();
	TimeSince lastCalculatedPath;
	TimeSince startedStalking;
	TimeSince startedFollowing;
	TimeSince stuckOnMovement;

	public virtual void ComputeAI()
	{
		if ( Game.State is not GameplayState )
			return;

		if ( Disabled ) return;

		ComputeAnimation();

		if ( stuckOnMovement >= 4f )
		{

			Position = ClosestNodeTo( NextPosition ).WorldPosition;
			stuckOnMovement = 0f;

		}


		/*DebugOverlay.Sphere( Position, JumpscareDistance, Color.Red, 0f, false );
		DebugOverlay.Sphere( Position, DetectDistance, Color.Green );
		DebugOverlay.Sphere( Position, StalkingDistance, Color.Yellow );
		DebugOverlay.Sphere( Position, AttackDistance, Color.Orange );
		DebugOverlay.Sphere( Position, GiveUpDistance, Color.Blue );*/

		if ( CurrentState != PolewikState.Idle && CurrentState != PolewikState.Pain )
		{

			ComputeMovement();

		}

		if ( CurrentState == PolewikState.Patrolling && PatrolPath != null )
		{

			if ( ClosestPlayer.Position.Distance( Position ) <= DetectDistance )
			{

				Victim = ClosestPlayer;
				CurrentState = PolewikState.Stalking;

			}

			if ( lastAttack >= AttackAfterStalling )
			{

				Victim = ClosestPlayer;
				CurrentState = PolewikState.Yell;

			}

			if ( Position.Distance( TargetPosition ) >= 30f )
			{

				if ( Velocity.Length >= 120f )
				{

					stuckOnMovement = 0f;

				}

			}

		}

		if ( CurrentState == PolewikState.Stalking && PatrolPath != null )
		{

			if ( Victim != null )
			{

				if ( lastCalculatedPath >= 0.5f )
				{

					if ( Position.Distance( TargetPosition ) >= 20f )
					{

						NavigateTo( ClosestNodeTo( ClosestPlayer.Position ).WorldPosition );

						if ( Velocity.Length >= 120f )
						{

							stuckOnMovement = 0f;

						}

					}
					else
					{

						WishRotation = Rotation.LookAt( Victim.Position );
						stuckOnMovement = 0f;

					}

				}

				if ( Victim.Position.Distance( Position ) <= StalkingDistance || startedStalking >= AttackAfterStalking )
				{

					PlaySound( "sounds/polewik/scream_scare.sound" );
					CurrentState = PolewikState.Following;

				}

				if ( PathLength >= GiveUpDistance || Math.Abs( Victim.Position.z - Position.z ) > 200f )
				{

					CurrentState = PolewikState.Fleeing;

				}

				var trace = Trace.Sphere( 200f, Victim.EyePosition, Victim.EyePosition + Victim.EyeRotation.Forward * 2000f )
					.WithTag("Polewik")
					.EntitiesOnly()
					.Ignore( Victim )
					.Run();

				if ( trace.Entity == this )
				{

					CurrentState = PolewikState.Following;

				}

			}
			else
			{

				CurrentState = PolewikState.Patrolling;

			}

		}

		if ( CurrentState == PolewikState.Following && Victim != null && PatrolPath != null )
		{

			if ( lastCalculatedPath >= 0.2f )
			{

				NavigateTo( Victim.Position );

			}

			if ( startedFollowing >= GiveUpAfter )
			{

				CurrentState = PolewikState.Patrolling;

			}

			if ( Victim.Position.Distance( Position ) <= AttackDistance && Math.Abs( Victim.Position.z - Position.z ) <= 200f )
			{

				CurrentState = PolewikState.Attacking;

			}

			if ( PathLength >= GiveUpDistance || Math.Abs( Victim.Position.z - Position.z ) > 200f )
			{

				CurrentState = PolewikState.Fleeing;

			}

			if ( Position.Distance( TargetPosition ) >= 20f )
			{

				if ( Velocity.Length >= 120f )
				{

					stuckOnMovement = 0f;

				}

			}

		}

		if ( CurrentState == PolewikState.AttackPersistent && Victim != null && PatrolPath != null )
		{

			if ( lastCalculatedPath >= 0.2f )
			{

				NavigateTo( Victim.Position );

			}

			if ( startedFollowing >= GiveUpAfter * 2f )
			{

				CurrentState = PolewikState.Patrolling;

			}

			if ( Victim.Position.Distance( Position ) <= AttackDistance && Math.Abs( Victim.Position.z - Position.z ) <= 200f )
			{

				CurrentState = PolewikState.Attacking;

			}

			if ( PathLength >= GiveUpDistance * 3f || Math.Abs( Victim.Position.z - Position.z ) > 200f )
			{

				CurrentState = PolewikState.Fleeing;

			}

			if ( Position.Distance( TargetPosition ) >= 20f )
			{

				if ( Velocity.Length >= 120f )
				{

					stuckOnMovement = 0f;

				}

			}

		}

		if ( CurrentState == PolewikState.Attacking && Victim != null )
		{

			if ( Victim.Position.Distance( Position ) <= 100f )
			{

				CurrentState = PolewikState.Jumpscare;


			}

			if ( PathLength >= GiveUpDistance )
			{

				CurrentState = PolewikState.Fleeing;

			}

			stuckOnMovement = 0f;

		}

		if ( CurrentState == PolewikState.Yell )
		{

			stuckOnMovement = 0f;

		}

		if ( CurrentState == PolewikState.Pain )
		{

			stuckOnMovement = 0f;

		}

		if ( CurrentState == PolewikState.Jumpscare )
		{

			stuckOnMovement = 0f;

		}

		if ( CurrentState == PolewikState.Fleeing && PatrolPath != null)
		{

			if ( Position.Distance( TargetPosition ) >= 30f )
			{

				if ( Velocity.Length >= 120f )
				{

					stuckOnMovement = 0f;

				}

			}

		}

	}

	public Rotation WishRotation;
	public Vector3 WishVelocity;

	public override void OnAnimEventFootstep( Vector3 position, int foot, float volume )
	{

		var trace = Trace.Ray( position, position + Vector3.Down * 10f )
			.Ignore( this )
			.Run();

		var surface = trace.Surface;
		var sound = surface.Sounds.FootLand;

		Sound.FromWorld( sound, trace.HitPosition )
			.SetVolume( 2 );

	}

	public virtual void ComputeAnimation()
	{
		if ( Disabled ) return;
		if ( !IsAuthority ) return;

		SetAnimParameter( "speed", Velocity.Length / 3 );

		if ( Victim != null )
		{

			var local = Transform.PointToLocal( Victim.Position );
			SetAnimParameter( "lookat", local.WithX( Math.Max( local.x, 0 ) ) + Vector3.Forward * 300f );

		}

		Rotation = Rotation.Lerp( Rotation, WishRotation, 5f * Time.Delta );

	}

	public Vector3[] PathPoints;
	public int PathIndex;
	public Vector3 NextPosition;

	public virtual void ComputeMovement()
	{

		if ( !IsAuthority ) return;

		if ( PathPoints != null )
		{

			ComputePath();

		}

		Velocity = Vector3.Lerp( Velocity, WishVelocity.WithZ( Velocity.z ), Time.Delta * 3f );

		if ( Velocity.Length > 0 )
		{

			var hindTrace = Trace.Ray( Transform.PointToWorld( new Vector3( 20f, 0f, 20f ) ), Transform.PointToWorld( new Vector3( 20f, 0f, -3000f ) ) )
				.Size( 5f )
				.Ignore( this )
				.Run();

			var backTrace = Trace.Ray( Transform.PointToWorld( new Vector3( -30f, 0f, 20f ) ), Transform.PointToWorld( new Vector3( -30f, 0f, -3000f ) ) )
				.Size( 5f )
				.Ignore( this )
				.Run();

			var standAngles = ( hindTrace.HitPosition - backTrace.HitPosition ).Normal.EulerAngles
				.WithRoll( 0 )
				.WithYaw( Velocity.WithZ( 0 ).EulerAngles.yaw );

			WishRotation = Rotation.From( standAngles );

		}

		ComputeMoveHelper();

	}

	public Vector3 GroundPosition;

	public virtual void ComputeMoveHelper()
	{

		var groundCheck = Trace.Box( CollisionBox, Position, Position + Vector3.Down * 5f )
			.Ignore( this )
			.Run();

		GroundEntity = groundCheck.Entity;
		GroundPosition = groundCheck.EndPosition;

		if ( GroundEntity == null )
		{

			Velocity += Vector3.Down * Time.Delta * Gravity;

		}
		else
		{

			Position = groundCheck.EndPosition;

		}


		var helper = new MoveHelper( Position, Velocity )
		{
			MaxStandableAngle = 40f // ATV Monster LOL!!!
		};

		helper.Trace = helper.Trace.Size( CollisionBox )
			.Ignore( this )
			.WithoutTags( "player" );
		helper.TryUnstuck();
		helper.TryMoveWithStep( Time.Delta, 30f );

		Position = helper.Position;
		Velocity = helper.Velocity;

	}

	public virtual bool NavigateTo( Vector3 pos )
	{

		var path = NavMesh.PathBuilder( GroundPosition )
			.WithPartialPaths()
			.WithNoOptimization()
			.WithAgentHull( Agent )
			.Build( pos );

		if ( path == null ) return false;

		foreach ( var point in path.Segments )
		{

			//DebugOverlay.Sphere( point.Position, 5f, Color.Blue, 0.5f, false );

		}

		CurrentPath = path;

		PathLength = 0f;

		for ( int i = 1; i < path.Segments.Count; i++ )
		{

			PathLength += path.Segments[i - 1].Position.Distance( path.Segments[i].Position );

		}

		PathIndex = 0;
		PathPoints = path.Segments.Select( segment => segment.Position ).ToArray();

		TargetPosition = pos;
		ReachedTarget = false;

		lastCalculatedPath = 0f;

		return true;

	}

	public virtual void ComputePath()
	{

		if ( PathPoints == null ) return;

		NextPosition = PathPoints[Math.Clamp( PathIndex + 1, 0, PathPoints.Length - 1 )];

		if ( Position.Distance( NextPosition ) < Math.Max( Velocity.Length, 100f ) * 10f * Time.Delta )
		{
			PathIndex++;

			if ( PathIndex == PathPoints.Length )
			{

				PathPoints = null;
				WishVelocity = 0;
				ReachedTarget = true;
				return;

			}

		}

		WishVelocity = (NextPosition.WithZ( 0 ) - Position.WithZ( 0 )).Normal * CurrentSpeed;

		if ( GroundEntity == null )
		{

			WishVelocity += Vector3.Down * Gravity * Time.Delta;

		}

	}
	public ModelEntity RagdollModel( ModelEntity modelEnt )
	{
		var ent = new ModelEntity();
		ent.Position = modelEnt.Position;
		ent.Rotation = modelEnt.Rotation;
		ent.Scale = modelEnt.Scale;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.SetModel( modelEnt.GetModelName() );
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		ent.CopyBonesFrom( modelEnt );
		ent.CopyBodyGroups( modelEnt );
		ent.CopyMaterialGroup( modelEnt );
		ent.TakeDecalsFrom( modelEnt );
		ent.CopyMaterialOverrides( modelEnt );
		ent.EnableHitboxes = true;
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ent.RenderColor = modelEnt.RenderColor;
		ent.PhysicsGroup.Velocity = modelEnt.Velocity;

		return ent;
	}


	public void OncurrentStateChanged( PolewikState oldState, PolewikState newState )
	{
	
	}

	public BasePathNode ClosestNodeTo( Vector3 pos )
	{

		return PatrolPath.PathNodes.OrderBy( x => x.WorldPosition.Distance( pos ) ).FirstOrDefault();

	}

	public BasePathNode NearestNode => ClosestNodeTo( Position );
	public BasePathNode FurthestNode => IsValid ? PatrolPath.PathNodes.OrderBy( x => x.WorldPosition.Distance( Position ) ).LastOrDefault() : null;

	[Event.Tick.Server]
	private void computeAI()
	{

		if ( !IsValid )
		{
			Delete();
			return;
		}

		if ( Disabled ) return;

		ComputeAI();

		if ( CurrentState == PolewikState.Patrolling || CurrentState == PolewikState.Fleeing )
		{

			if ( ReachedTarget )
			{

				if ( PatrolPath != null )
				{

					CurrentPathId = (CurrentPathId + 1) % PatrolPath.PathNodes.Count;

					NavigateTo( PatrolPath.PathNodes[CurrentPathId].WorldPosition );

				}

			}

		}

	}

}
