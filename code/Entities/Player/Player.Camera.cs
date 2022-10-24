namespace SpookyJam2022;

public partial class Player
{

	ScriptedEventCamera overrideCamera;
	ScriptedEventTrigger overrideTrigger;
	bool scriptedEvent = false;

	float walkBob = 0f;

	public override void PostCameraSetup( ref CameraSetup setup )
	{

		Player pawn = Client.Pawn as Player;

		var speed = pawn.Velocity.Length / 160f;
		var left = setup.Rotation.Left;
		var up = setup.Rotation.Up;

		if ( pawn.GroundEntity != null )
		{
			walkBob += Time.Delta * speed * 20f;
		}

		var upOffset = up * MathF.Sin( walkBob ) * speed * -2;
		var sideOffset = left * MathF.Sin( walkBob * 0.5f ) * speed * -3f;

		if ( scriptedEvent )
		{

			setup.Position = Vector3.Lerp( setup.Position, overrideCamera.Position, Time.Delta * overrideTrigger.TransitionSpeed );
			setup.Rotation = Rotation.Lerp( setup.Rotation, overrideCamera.Rotation, Time.Delta * overrideTrigger.TransitionSpeed );

		}
		else
		{

			setup.Position = Vector3.Lerp( setup.Position - upOffset - sideOffset, EyePosition, Time.Delta * 30f );
			setup.Rotation = Rotation.Lerp( setup.Rotation, EyeRotation, Time.Delta * 30f );

		}

		setup.Position += upOffset;
		setup.Position += sideOffset;
		setup.FieldOfView = 70;

	}

	[Event( "ScriptedEventStart" )]
	void startScriptedEvent( string name, ScriptedEventTrigger trigger )
	{

		scriptedEvent = true;

		foreach ( var ent in Entity.All ) // FindByName doesn't work????
		{

			if ( ent is not ScriptedEventCamera camera ) continue;

			if ( camera.Name == name )
			{

				overrideCamera = camera;

			}

		}

		overrideTrigger = trigger;

	}

	[Event( "ScriptedEventEnd")]
	void endScriptedEvent()
	{

		scriptedEvent = false;

	}

}
