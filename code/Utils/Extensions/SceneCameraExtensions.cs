namespace SpookyJam2022;

public static partial class Extensions
{
	public class SceneRender
	{
		public Texture Texture { get; private set; }
		public SceneCamera Camera { get; private set; }

		public SceneRender( SceneCamera camera, Texture renderTarget )
		{
			Texture = renderTarget;
			Camera = camera;
		}
	}

	private static List<SceneRender> renderQueue = new();

	public static SceneRender Render( this SceneCamera camera, Vector2 size )
	{
		var result = new SceneRender( 
			camera, 
			Texture.CreateRenderTarget( "sceneRender", ImageFormat.RGBA8888, size ) 
		);
		renderQueue.Add( result );
		return result;
	}

	[Event( "Render" )]
	private static void render()
	{
		for ( int i = 0; i < renderQueue.Count; i++ )
		{
			var render = renderQueue[i];
			if ( render == null || render.Texture == null )
			{
				renderQueue.RemoveAt( i );
				continue;
			}

			Graphics.RenderToTexture( render.Camera, render.Texture );
			renderQueue.RemoveAt( i );
		}
	}
}
