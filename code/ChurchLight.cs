using Sandbox;

public sealed class ChurchLight : Component
{
	[Property]
	public List<SpotLight> Lights { get; set; }

	[Property]
	public Color Color { get; set; }

	protected override void OnFixedUpdate()
	{
		var camera = Scene.Camera;
		if ( !camera.IsValid() ) return;

		foreach ( var light in Lights )
		{
			var distance = light.WorldPosition.Distance( camera.WorldPosition );
			var intensity = MathX.Remap( distance, 1800f, 5000f, 0.02f, 1f );

			light.LightColor = Color.Black.LerpTo( Color, intensity );
		}
	}
}
