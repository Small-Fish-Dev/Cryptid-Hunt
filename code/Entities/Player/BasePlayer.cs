using Sandbox;

namespace SpookyJam2022;

public abstract partial class BasePlayer : AnimatedEntity
{
	public Vector3 EyeLocalPosition = new Vector3( 0f, 0f, 64f );
	public Vector3 EyePosition => Position + EyeLocalPosition;
	public Rotation EyeRotation => InputLook.ToRotation();
	[ClientInput] public Vector3 InputDirection { get; set; }
	[ClientInput] public Angles InputLook { get; set; }

	public override void BuildInput()
	{
		if ( Input.Down( InputButton.SecondaryAttack ) )
		{
			InputLook += Input.AnalogLook;
		}
		InputDirection = Input.AnalogMove;
	}
	public override void Spawn()
	{
		base.Spawn();
		
		Tags.Add( "player" );
	}

	public abstract void Respawn();

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		Camera.Rotation = EyeRotation;
		Camera.Position = EyePosition;
		Camera.FieldOfView = Game.Preferences.FieldOfView;
		Camera.ZFar = 3000.0f;
	}
}
