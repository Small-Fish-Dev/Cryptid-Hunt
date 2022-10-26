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

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 1f );

					CurrentState = PolewikState.Fleeing;

				} );

			}

			if ( value == PolewikState.Yell )
			{

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 1f );

					Victim = ClosestPlayer;
					CurrentState = PolewikState.AttackPersistent;

				} );

			}

			if ( value == PolewikState.Fleeing )
			{

				NavigateTo( FurthestNode.WorldPosition );
				CurrentPathId = PatrolPath.PathNodes.IndexOf( FurthestNode );

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( Rand.Float( 15f, 30f ) );

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

			if ( value == PolewikState.Attacking )
			{

				SetAnimParameter( "leap", true );

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 0.05f );

					SetAnimParameter( "leap", false );

					await GameTask.DelaySeconds( 0.9f );

					if ( CurrentState  == PolewikState.Attacking )
					{

						CurrentState = PolewikState.Following;

					}

				} );

			}

		}
	}
	public GenericPathEntity PatrolPath => Entity.All.OfType<GenericPathEntity>().Where( x => x.Name == "Monster").FirstOrDefault();
	public string ModelName => "models/polewik/polewik.vmdl";
	public float Gravity => 700f;
	public Dictionary<PolewikState, float> Speeds = new()
	{
		{ PolewikState.Idle, 0f },
		{ PolewikState.Patrolling, 300f },
		{ PolewikState.Stalking, 300f },
		{ PolewikState.Following, 300f },
		{ PolewikState.Attacking, 1800f },
		{ PolewikState.Fleeing, 450f },
		{ PolewikState.Pain, 0f },
		{ PolewikState.Yell, 0f },
		{ PolewikState.AttackPersistent, 300f },
		{ PolewikState.Jumpscare, 0f }

	};
	public float CurrentSpeed => Speeds[CurrentState];
	public NavAgentHull Agent => NavAgentHull.Agent1;
	public Vector3 TargetPosition { get; set; }
	public bool ReachedTarget { get; set; } = true;
	public int CurrentPathId { get; set; } = 0;
	public NavPath CurrentPath;
	public float PathLength;

	public BBox CollisionBox;

	[Net] public float HP { get; set; } = 100f;

	[Net] bool disabled { get; set; } = false;
	public bool Disabled
	{
		get { return disabled; }
		set
		{
			disabled = value;

			EnableDrawing = value;
			EnableAllCollisions = value;
			UseAnimGraph = value;
			EnableShadowCasting = value;
			Transmit = value ? TransmitType.Never : TransmitType.Pvs;

		}
	}

	public Player Victim;

	public override void Spawn()
	{

		base.Spawn();

		SetModel( ModelName );

		CollisionBox = new BBox( new Vector3( -40f, -20f, 1f ), new Vector3( 20f, 20f, 70f ) );

		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, CollisionBox.Mins, CollisionBox.Maxs );

	}

	public Player ClosestPlayer => Entity.All.OfType<Player>().OrderBy( x => x.Position.Distance( this.Position ) ).FirstOrDefault();
	TimeSince lastCalculatedPath;
	TimeSince startedStalking;
	TimeSince startedFollowing;
	TimeSince startedAttacking;

	public virtual void ComputeAI()
	{

		if ( Disabled ) return;

		ComputeAnimation();

		if ( CurrentState != PolewikState.Idle && CurrentState != PolewikState.Pain )
		{

			ComputeMovement();

		}

		if ( CurrentState == PolewikState.Patrolling && PatrolPath != null )
		{

			if ( ClosestPlayer.Position.Distance( Position ) <= 2000f )
			{

				Victim = ClosestPlayer;
				CurrentState = PolewikState.Stalking;

			}

		}

		if ( CurrentState == PolewikState.Stalking && PatrolPath != null )
		{

			if ( Victim != null )
			{

				if ( lastCalculatedPath >= 2f )
				{

					NavigateTo( ClosestNodeTo( ClosestPlayer.Position ).WorldPosition );

				}

				if ( Victim.Position.Distance( Position ) <= 1000f || startedStalking >= 45f )
				{

					CurrentState = PolewikState.Following;

				}

				if ( PathLength >= 2500f )
				{

					CurrentState = PolewikState.Fleeing;

				}

			}
			else
			{

				CurrentState = PolewikState.Patrolling;

			}

		}

		if ( CurrentState == PolewikState.Following && Victim != null && PatrolPath != null )
		{

			if ( lastCalculatedPath >= 0.5f )
			{

				NavigateTo( Victim.Position );

			}

			if ( startedFollowing >= 15f )
			{

				CurrentState = PolewikState.Patrolling;

			}

			if ( Victim.Position.Distance( Position ) <= 600f && Math.Abs( Victim.Position.z - Position.z ) <= 400f )
			{

				CurrentState = PolewikState.Attacking;

			}

			if ( PathLength >= 2500f )
			{

				CurrentState = PolewikState.Fleeing;

			}

		}

		if ( CurrentState == PolewikState.Attacking && Victim != null )
		{

			if ( Victim.Position.Distance( Position ) <= 100f )
			{

				Victim.HP -= 1;

				CurrentState = PolewikState.Fleeing;

			}

			if ( PathLength >= 2500f )
			{

				CurrentState = PolewikState.Fleeing;

			}

		}

	}

	public Rotation WishRotation;
	public Vector3 WishVelocity;

	public virtual void ComputeAnimation()
	{

		if ( Disabled ) return;
		if ( !IsAuthority ) return;

		SetAnimParameter( "speed", Velocity.Length / 3 );

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

	public virtual void ComputeMoveHelper()
	{

		var groundCheck = Trace.Box( CollisionBox, Position, Position + Vector3.Down )
			.Ignore( this )
			.Run();

		GroundEntity = groundCheck.Entity;

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
			MaxStandableAngle = 60f
		};

		helper.Trace = helper.Trace.Size( CollisionBox )
			.Ignore( this )
			.WithoutTags( "player" );
		helper.TryUnstuck();
		helper.TryMoveWithStep( Time.Delta, 32f );

		Position = helper.Position;
		Velocity = helper.Velocity;

	}

	public virtual bool NavigateTo( Vector3 pos )
	{

		var path = NavMesh.PathBuilder( Position )
			.WithAgentHull( Agent )
			.Build( pos );

		if ( path == null ) return false;

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

		if ( Position.Distance( NextPosition ) < Velocity.Length * 10f * Time.Delta )
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

	public void OncurrentStateChanged( PolewikState oldState, PolewikState newState )
	{
	
	}

	public BasePathNode ClosestNodeTo( Vector3 pos )
	{

		return PatrolPath.PathNodes.OrderBy( x => x.WorldPosition.Distance( pos ) ).FirstOrDefault();

	}

	public BasePathNode NearestNode => ClosestNodeTo( Position );
	public BasePathNode FurthestNode => PatrolPath.PathNodes.OrderBy( x => x.WorldPosition.Distance( Position ) ).LastOrDefault();

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

		if ( CurrentState == PolewikState.Patrolling )
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
