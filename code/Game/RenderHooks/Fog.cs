namespace SpookyJam2022;

[SceneCamera.AutomaticRenderHook]
public class Fog : RenderHook
{
	RenderAttributes attributes = new RenderAttributes();

	public float Radius { get; set; } = 800f;
	public Color Color { get; set; } = Color.Black;

	public override void OnStage( SceneCamera target, Stage stage )
	{
		if ( stage != Stage.BeforePostProcess  ) return;

		var material = Material.FromShader( "Fog.vfx" );

		attributes.Set( "fogColor", Color );
		attributes.Set( "fogRadius", Radius );

		Graphics.GrabFrameTexture( "ColorBuffer", attributes );
		Graphics.GrabDepthTexture( "DepthBuffer", attributes );

		Graphics.Blit( material, attributes );
	}
}
