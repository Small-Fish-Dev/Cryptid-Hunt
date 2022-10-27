namespace SpookyJam2022;


[HammerEntity]
[EditorModel( "models/polewik/polewik.vmdl" )]
[Display( Name = "Polewik", GroupName = "Monster", Description = "the monster" )]
public partial class NextBot : Entity
{

	public float Speed => 300f;
	public NavAgentHull Agent => NavAgentHull.Agent1;


	[Net] public bool Disabled { get; set; } = false;

	public Player Target => Disabled ? null : Entity.All.OfType<Player>().OrderBy( x => x.Position.Distance( Position ) ).FirstOrDefault();

	[Event.Tick.Server]
	public void ComputeAI()
	{

		if ( Disabled ) return;
		if ( Target == null ) return;

		ComputeMovement();

		if ( Target.Position.Distance( Position ) <= 200f )
		{

			// Kill

		}

	}

	public Vector3[] PathPoints;
	public int PathIndex;
	public Vector3 NextPosition;
	TimeSince lastCalculatedPath;

	public void ComputeMovement()
	{

		if ( PathPoints != null )
		{

			ComputePath();

		}

		if ( lastCalculatedPath <= 0.2f )
		{

			NavigateTo( Target.Position );

		}

	}

	public virtual bool NavigateTo( Vector3 pos )
	{

		var path = NavMesh.PathBuilder( Position )
			.WithAgentHull( Agent )
			.Build( pos );

		if ( path == null ) return false;

		PathIndex = 0;
		PathPoints = path.Segments.Select( segment => segment.Position ).ToArray();

		lastCalculatedPath = 0f;

		return true;

	}

	public virtual void ComputePath()
	{

		if ( PathPoints == null ) return;

		NextPosition = PathPoints[Math.Clamp( PathIndex + 1, 0, PathPoints.Length - 1 )];

		if ( Position.Distance( NextPosition ) <= Speed * Time.Delta )
		{

			PathIndex++;

			if ( PathIndex == PathPoints.Length )
			{

				PathPoints = null;
				return;

			}

		}

		var dir = (NextPosition - Position).Normal;

		Position += dir * Speed * Time.Delta;

	}

	[Event("SetImage")]
	public async void SetImage( string prompt )
	{

		// Load the image here
		// Await...
		// Done loading

		Disabled = false;

	}

}
