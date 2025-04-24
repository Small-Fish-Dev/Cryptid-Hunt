namespace CryptidHunt;

public sealed class Flickerer : Component
{
	[Property]
	public List<Light> Lights { get; set; }

	[Property]
	public Color Color { get; set; }


	protected override void OnFixedUpdate()
	{
		foreach ( var light in Lights )
		{
			float noise = Noise.Perlin( Time.Now * 100f, 0f );

			light.LightColor = Color.Black.LerpTo( Color, noise );
		}
	}
}
