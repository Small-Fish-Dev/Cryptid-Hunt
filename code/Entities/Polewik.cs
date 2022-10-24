namespace SpookyJam2022;

public enum PolewikState
{
	Idle,
	Patrolling,
	Stalking,
	Following,
	Attacking
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

		}
	}
	[Net] public GenericPathEntity PatrolPath { get; set; } = Entity.All.OfType<GenericPathEntity>().Where( x => x.Name == "Monster").FirstOrDefault();
	public virtual string ModelName => "models/polewik/polewik.vmdl";
	public virtual Vector3 Mins => new Vector3( -30f, -30f, 0f );
	public virtual Vector3 Maxs => new Vector3( 30f, 30f, 70f );
	public virtual float MoveSpeed => 300f;
	public virtual float SprintSpeed => 450f;
	public virtual float Gravity => 700f;
	public virtual NavAgentHull Agent => NavAgentHull.Agent1;
	public virtual Vector3 TargetPosition { get; set; }
	public bool ReachedTarget { get; set; } = true;
	public int CurrentPathId { get; set; } = 0;

	public BBox CollisionBox;

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

	public override void Spawn()
	{

		base.Spawn();

		SetModel( ModelName );

		CollisionBox = new BBox( Mins, Maxs );

		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, Mins, Maxs );

	}

	public virtual void ComputeAI()
	{

		if ( Disabled ) return;

		ComputeAnimation();
		ComputeMovement();

	}

	public Rotation WishRotation;
	public Vector3 WishVelocity;

	public virtual void ComputeAnimation()
	{

		if ( Disabled ) return;

		var animHelper = new CitizenAnimationHelper( this );
		animHelper.WithVelocity( Velocity );

		if ( !IsAuthority ) return;

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
			WishRotation = Rotation.LookAt( Velocity );
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

		PathIndex = 0;
		PathPoints = path.Segments.Select( segment => segment.Position ).ToArray();

		TargetPosition = pos;
		ReachedTarget = false;

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

		WishVelocity = (NextPosition.WithZ( 0 ) - Position.WithZ( 0 )).Normal * MoveSpeed;

		if ( GroundEntity == null )
		{

			WishVelocity += Vector3.Down * Gravity * Time.Delta;

		}

	}

	public void OncurrentStateChanged( PolewikState oldState, PolewikState newState )
	{
	
	}

	public BasePathNode NearestNode => PatrolPath.PathNodes.OrderBy( x => x.Position.Distance( Position ) ).FirstOrDefault();

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
