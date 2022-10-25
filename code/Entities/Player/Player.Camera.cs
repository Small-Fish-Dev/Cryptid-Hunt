namespace SpookyJam2022;

public partial class Player
{

	[Net] public ScriptedEventCamera OverrideCamera { get; set; }
	[Net] public ScriptedEventTrigger OverrideTrigger { get; set; }
	[Net] public bool ScriptedEvent { get; set; }

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

		var posDiff = Vector3.Zero;
		var rotDiff = new Rotation();

		if ( ScriptedEvent )
		{

			if ( OverrideTrigger != null )
			{

				setup.Position = Vector3.Lerp( setup.Position, OverrideCamera.Position, Time.Delta * OverrideTrigger.TransitionSpeed );
				setup.Rotation = Rotation.Lerp( setup.Rotation, OverrideCamera.Rotation, Time.Delta * OverrideTrigger.TransitionSpeed );

			}
			else
			{

				setup.Position = OverrideCamera.Position;
				setup.Rotation = OverrideCamera.Rotation;

			}

		}
		else
		{

			var newPos = Vector3.Lerp( setup.Position - upOffset - sideOffset, EyePosition, Time.Delta * 30f );
			posDiff = EyePosition - newPos;
			var newRot = Rotation.Lerp( setup.Rotation, EyeRotation, Time.Delta * 30f );
			rotDiff = EyeRotation - newRot;

			setup.Position = newPos;
			setup.Rotation = newRot;

		}

		setup.Position += upOffset;
		setup.Position += sideOffset;

		posDiff += upOffset;
		posDiff += sideOffset;

		setup.FieldOfView = 70;

		Event.Run( "PostCameraSetup", posDiff, rotDiff );

	}

	[Event( "ScriptedEventStart" )]
	void startScriptedEvent( string name, ScriptedEventTrigger trigger )
	{

		ScriptedEvent = true;

		foreach ( var ent in Entity.All ) // FindByName doesn't work????
		{

			if ( ent is not ScriptedEventCamera camera ) continue;

			if ( camera.Name == name )
			{

				OverrideCamera = camera;

			}

		}

		OverrideTrigger = trigger;

	}

	[Event( "ScriptedEventEnd")]
	void endScriptedEvent()
	{

		ScriptedEvent = false;

	}

}
