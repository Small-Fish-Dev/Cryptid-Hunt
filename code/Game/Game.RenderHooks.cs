namespace SpookyJam2022;

public partial class Game
{
	public static ScreenEffects ScreenEffects { get; private set; }

	[Event( "GameStart" )]
	private void initialize()
	{
		if ( Host.IsServer ) return;

		ScreenEffects = Map.Camera.FindOrCreateHook<ScreenEffects>();
		ScreenEffects.Vignette.Smoothness = 1f;
		ScreenEffects.Vignette.Intensity = 0.72f;
		ScreenEffects.Vignette.Roundness = 0.1f;

		// FIX: disabled till fixed
		/*ScreenEffects.MotionBlur.Scale = 0.05f;
		ScreenEffects.MotionBlur.Samples = 8;*/

		ScreenEffects.FilmGrain.Intensity = 0.01f;
		ScreenEffects.FilmGrain.Response = 0.5f;
	}

	[Event.Hotload]
	private void hotload() => initialize();
}
