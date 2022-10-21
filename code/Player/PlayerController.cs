namespace SpookyJam2022;

public partial class PlayerController : PawnController
{
	[Net] public float MoveSpeed { get; set; } = 200f;

	private Vector3 gravity => new Vector3( 0, 0, -700f );
	private float stepSize => 16f;

	public override void Simulate()
	{
		Pawn.EyePosition = Position + EyeLocalPosition;
		EyeRotation = Input.Rotation;

		var wishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
		wishVelocity = wishVelocity
			* wishVelocity.Length.Clamp( 0, 1 )
			* Rotation.FromYaw( Input.Rotation.Yaw() )
			* MoveSpeed;

		Velocity = wishVelocity.WithZ( Velocity.z );

		if ( GroundEntity == null )
			Velocity += gravity * Time.Delta;
		else
		{
			// jump ? i think
		}

		var helper = new MoveHelper( Position, Velocity )
		{
			MaxStandableAngle = 60f
		};
		helper.Trace = helper.Trace.Size( Player.CollisionBox.Mins, Player.CollisionBox.Maxs )
			.Ignore( Pawn );
		helper.TryUnstuck();
		helper.TryMoveWithStep( Time.Delta, stepSize );

		Position = helper.Position;
		Velocity = helper.Velocity;

		if ( Velocity.z <= stepSize )
		{
			var tr = helper.TraceDirection( Vector3.Down * 2f );
			GroundEntity = tr.Entity;

			if ( tr.Entity != null )
			{
				Position += tr.Distance * Vector3.Down;

				if ( Velocity.z < 0f ) 
					Velocity = Velocity.WithZ( 0f );
			}
		}
		else GroundEntity = null;
	}

	public override void FrameSimulate()
	{
		Pawn.EyePosition = Position + EyeLocalPosition;
		EyeRotation = Input.Rotation;
	}
}
