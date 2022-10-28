namespace SpookyJam2022;

public static partial class Extensions
{
	public class SceneRender
	{
		public Texture Texture { get; private set; }
		public SceneCamera Camera { get; private set; }
		public bool RenderOnce { get; private set; }
		public int Framerate { get; private set; }

		public float FrameTime { get; set; } = 0f;
		public bool Deleting { get; private set; }

		public SceneRender( SceneCamera camera, Texture renderTarget, bool renderOnce, int targetFramerate )
		{
			Texture = renderTarget;
			Camera = camera;
			RenderOnce = renderOnce;
			Framerate = targetFramerate;
		}

		public void Delete()
		{
			Deleting = true;
		}
	}

	private static List<SceneRender> renderQueue = new();

	public static SceneRender Render( this SceneCamera camera, Vector2 size, bool renderOnce = true, int targetFramerate = 30 )
	{
		var result = new SceneRender( 
			camera, 
			Texture.CreateRenderTarget( "sceneRender", ImageFormat.RGBA8888, size ),
			renderOnce,
			targetFramerate
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

			if ( render.RenderOnce || render.FrameTime > 1f / render.Framerate )
			{
				Graphics.RenderToTexture( render.Camera, render.Texture );
				render.FrameTime = 0;
			}

			render.FrameTime += Time.Delta;

			if ( render.RenderOnce || render.Deleting )
				renderQueue.RemoveAt( i );
		}
	}
}
