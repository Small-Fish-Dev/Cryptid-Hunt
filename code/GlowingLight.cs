namespace CryptidHunt;

public sealed class GlowingLight : Component
{
	[Property]
	public List<Light> Lights { get; set; }

	[Property]
	public Color Color { get; set; }

	protected override void OnEnabled()
	{
		base.OnEnabled();

		foreach ( var light in Lights )
		{
			light.LightColor = Color.Black;
		}
	}

	protected override void OnFixedUpdate()
	{
		foreach ( var light in Lights )
		{
			float noise = (Noise.Perlin( Time.Now * 60f, 0f ) - 0.3f) * 2f;
			var color = Color.Black.LerpTo( Color, noise );

			light.LightColor = light.LightColor.LerpTo( color, Time.Delta * 5f );
		}
	}
}
