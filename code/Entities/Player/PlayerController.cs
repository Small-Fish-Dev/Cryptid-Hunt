namespace SpookyJam2022;

public partial class PlayerController : PawnController
{
	[Net] public float MoveSpeed { get; set; } = 80f;
	[Net] public float SprintSpeed { get; set; } = 160f;

	private Vector3 gravity => new Vector3( 0, 0, -700f );
	private float stepSize => 16f;

	TimeSince lastStep = 0f;

	public override void Simulate()
	{
		EyeRotation = Input.Rotation;

		var wishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
		wishVelocity = wishVelocity.Normal
			* wishVelocity.Length.Clamp( 0, 1 )
			* Rotation.FromYaw( Input.Rotation.Yaw() )
			* ( Input.Down( InputButton.Run ) ? SprintSpeed : MoveSpeed );

		Velocity = Vector3.Lerp( Velocity, wishVelocity, 12f * Time.Delta )
			.WithZ( Velocity.z );

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

		if ( Host.IsClient ) return;

		if ( Velocity.Length > 0f && lastStep >= 50 / Velocity.Length && GroundEntity != null )
		{

			var trace = Trace.Ray( Position, Position + Vector3.Down * 10f )
				.Ignore( Pawn )
				.Run();

			var surface = trace.Surface;
			var sound = surface.Sounds.FootLand;

			Sound.FromEntity( sound, Pawn ).SetVolume( Velocity.Length / 80f );
			lastStep = 0f;

		}

	}

	public override void FrameSimulate()
	{
		
	}
}
