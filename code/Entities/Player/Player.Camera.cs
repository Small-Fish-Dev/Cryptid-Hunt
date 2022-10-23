namespace SpookyJam2022;

public partial class Player
{

	ScriptedEventCamera overrideCamera;
	ScriptedEventTrigger overrideTrigger;
	bool scriptedEvent = false;

	public override void PostCameraSetup( ref CameraSetup setup )
	{

		if ( scriptedEvent )
		{

			setup.Position = Vector3.Lerp( setup.Position, overrideCamera.Position, Time.Delta * overrideTrigger.TransitionSpeed );
			setup.Rotation = Rotation.Lerp( setup.Rotation, overrideCamera.Rotation, Time.Delta * overrideTrigger.TransitionSpeed );

		}
		else
		{

			setup.Position = Vector3.Lerp( setup.Position, EyePosition, Time.Delta * 30f );
			setup.Rotation = Rotation.Lerp( setup.Rotation, EyeRotation, Time.Delta * 30f );

		}

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
