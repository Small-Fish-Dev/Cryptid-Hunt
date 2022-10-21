using static Sandbox.Event;

namespace SpookyJam2022;

public partial class Player : AnimatedEntity
{
	private static Vector3 mins = new Vector3( -16f, -16f, 0f );
	private static Vector3 maxs = new Vector3( 16f, 16f, 72f );
	public static BBox CollisionBox = new BBox( mins, maxs );

	[Net, Predicted] public PawnController Controller { get; set; }

	public override void Spawn()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, mins, maxs );

		EyeLocalPosition = Vector3.Up * maxs.z;
		Controller ??= new PlayerController();

		Respawn();
	}

	public void Respawn()
	{
		EnableAllCollisions = true;
		EnableDrawing = true;
		
		Position = Vector3.Up * 300f;
		Rotation = Transform.Zero.Rotation;

		ResetInterpolation();
	}

	public override void Simulate( Client cl )
	{
		Controller?.Simulate( cl, this, null );
	}

	public override void FrameSimulate( Client cl )
	{
		Controller?.FrameSimulate( cl, this, null );
		EyePosition = Position + EyeLocalPosition;
		EyeRotation = Input.Rotation;
	}

	public override void BuildInput( InputBuilder input )
	{
		input.ViewAngles += input.AnalogLook;
		input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
		input.InputDirection = input.AnalogMove;
	}
}
