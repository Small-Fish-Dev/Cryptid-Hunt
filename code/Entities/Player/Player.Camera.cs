namespace SpookyJam2022;

public partial class Player
{

	[Net] public ScriptedEventCamera OverrideCamera { get; set; }
	[Net] public ScriptedEventTrigger OverrideTrigger { get; set; }
	[Net] public bool ScriptedEvent { get; set; }

	float walkBob = 0f;

	TimeUntil cameraShake = 0f;
	float shakeIntensity = 0f;
	Vector2 lastShake;

	public override void PostCameraSetup( ref CameraSetup setup )
	{

		if ( Local.Pawn is not Player pawn ) return;

		var speed = pawn.Velocity.Length / 160f;
		var left = setup.Rotation.Left;
		var up = setup.Rotation.Up;

		if ( pawn.GroundEntity != null )
		{
			walkBob += Time.Delta * speed * 20f;
		}

		var upOffset = ScriptedEvent ? 0 : ( up * ( MathF.Sin( walkBob ) * 0.75f * -2f ) );
		var sideOffset = ScriptedEvent ? 0 : ( left * ( MathF.Sin( walkBob * 0.5f ) * speed * -3f ) );

		var posDiff = Vector3.Zero;
		var rotDiff = new Rotation();

		if ( ScriptedEvent )
		{

			if ( OverrideTrigger != null && OverrideCamera != null )
			{

				setup.Position = Vector3.Lerp( setup.Position, OverrideCamera.Position, Time.Delta * OverrideTrigger.TransitionSpeed );
				setup.Rotation = Rotation.Lerp( setup.Rotation, OverrideCamera.Rotation, Time.Delta * OverrideTrigger.TransitionSpeed );

			}
			else if ( OverrideTrigger == null && OverrideCamera != null )
			{

				if ( OverrideCamera.Target != null )
				{

					setup.Position = OverrideCamera.DoNotLerp ? OverrideCamera.Position : Vector3.Lerp( setup.Position, OverrideCamera.Position, Time.Delta );
					setup.Rotation = OverrideCamera.DoNotLerp ? OverrideCamera.Rotation : Rotation.Lerp( setup.Rotation, OverrideCamera.Rotation, Time.Delta );
					setup.FieldOfView = 40f;

				}
				else
				{

					setup.Position = OverrideCamera.Position;
					setup.Rotation = OverrideCamera.Rotation;

				}

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


		if ( cameraShake > 0 )
		{

			lastShake = new Vector2( ( Noise.Perlin( Time.Now * shakeIntensity * 20f, 18924 ) * 2 - 1 ) * shakeIntensity, ( Noise.Perlin( Time.Now * shakeIntensity * 20f, 9124 ) * 2 - 1 ) * shakeIntensity );

		}
		else
		{

			lastShake = Vector2.Lerp( lastShake, 0f, Time.Delta * shakeIntensity );

		}

		setup.FieldOfView = 60f;
		setup.Viewer = pawn;

		Event.Run( "PostCameraSetup", posDiff, rotDiff );
		

	}

	[Event( "ScriptedEventStart" )]
	void startScriptedEvent( string name, ScriptedEventTrigger trigger )
	{

		foreach ( var ent in Entity.All ) // FindByName doesn't work????
		{

			if ( ent is not ScriptedEventCamera camera ) continue;

			if ( camera.Name == name || camera.Name.Contains( name ) )
			{

				OverrideCamera = camera;
				OverrideTrigger = trigger;
				ScriptedEvent = true;

			}

		}

	}

	[Event( "ScriptedEventEnd")]
	void endScriptedEvent()
	{

		ScriptedEvent = false;
		OverrideTrigger = null;
		OverrideCamera = null;

	}

	[Event("ScreenShake")]
	void addScreenShake( float duration, float intensity )
	{

		cameraShake = duration;
		shakeIntensity = intensity;

	}

}
