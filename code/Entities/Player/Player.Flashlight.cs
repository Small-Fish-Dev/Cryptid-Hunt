namespace SpookyJam2022;

public partial class Player
{

	public SpotLightEntity FlashLight { get; set; }

	private SpotLightEntity CreateLight() //thanks sandbox
	{
		var light = new SpotLightEntity
		{
			Enabled = true,
			DynamicShadows = true,
			Range = 512,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Brightness = 2,
			Color = Color.White,
			InnerConeAngle = 20,
			OuterConeAngle = 40,
			FogStrength = 1.0f,
			Owner = Owner,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )
		};

		return light;
	}

	[Event("PostCameraSetup")]
	void calcFlashLight( Vector3 posDiff, Rotation rotDiff )
	{

		FlashLight ??= CreateLight();
		FlashLight.Enabled = true;

		var endPos = EyePosition + EyeRotation.Forward * 30f + EyeRotation.Right * -10f + EyeRotation.Down * 10f;

		var trace = Trace.Ray( EyePosition, endPos )
			.Ignore( this )
			.Run();


		FlashLight.Position = trace.EndPosition - EyeRotation.Forward * 5f;
		FlashLight.Rotation = EyeRotation - rotDiff;


	}

}
