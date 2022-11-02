using System.Runtime.CompilerServices;

namespace SpookyJam2022;

public partial class Player
{

	public SpotLightEntity FlashLight { get; set; }
	[Net] public bool FlashLightOn { get; set; } = false;

	public SpotLightEntity CreateLight( Entity parent = null) //thanks sandbox
	{
		var light = new SpotLightEntity
		{
			Enabled = true,
			DynamicShadows = parent != null ? false : true,
			Range = parent != null ? 1024 : 2048,
			Brightness = parent != null ? 0.16f : 5f,
			Color = Color.White,
			InnerConeAngle = 20,
			OuterConeAngle = 40,
			FogStrength = 0.0f,
			Owner = Owner,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
		};

		light.Transmit = TransmitType.Always;

		if ( parent != null )
		{

			light.Position = parent.Position - parent.Rotation.Forward * 30f;
			light.Rotation = parent.Rotation;
			light.SetParent( parent );

		}

		return light;
	}

	[Event("PostCameraSetup")]
	void calcFlashLight( Vector3 posDiff, Rotation rotDiff )
	{

		FlashLight ??= CreateLight();
		FlashLight.Enabled = FlashLightOn;

		var endPos = EyePosition + EyeRotation.Forward * 30f + EyeRotation.Right * -10f + EyeRotation.Down * 10f;

		var trace = Trace.Ray( EyePosition, endPos )
			.Ignore( this )
			.Run();

		FlashLight.Position = trace.EndPosition - EyeRotation.Forward * 5f;
		FlashLight.Rotation = EyeRotation - rotDiff;


	}

	[Event("FlashLight")]
	public void SetFlashLight( bool on, bool sound = false )
	{

		if ( FlashLightOn != on )
		{

			FlashLightOn = on;

			if ( sound )
			{

				PlaySound( "sounds/ui/button_click.sound" );

			}

		}

	}

}
