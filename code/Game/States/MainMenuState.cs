using Sandbox.Internal;

namespace SpookyJam2022.States;

public partial class MainMenuState : BaseState
{

	PointLightEntity light;
	public override void Init()
	{
		NextBotState.Deaths = 0;
		
		ShowMenu(To.Everyone);

		var cameras = Entity.All.OfType<ScriptedEventCamera>().ToList(); // collection was modified error waaa waa

		for ( int i = 0; i < cameras.Count; i++ )
		{
			var camera = cameras[i];
			if ( camera == null ) continue;

			if ( camera.Name.Contains( "MainMenu" ) )
			{

				foreach ( var ply in Entity.All.OfType<Player>() )
				{
					ply.OverrideCamera = camera;
					ply.ScriptedEvent = true;
					ply.LockInputs = true;

				}

				light = new PointLightEntity();
				light.Position = camera.Position;
				light.SetLightBrightness( 1f );
				light.SetLightColor( new Color( 1f, 0.95f, 0.8f ) );
				light.Range = 3000;

			}

		}
	}
	
	public override void CleanUp()
	{
		// The menu hides itself.
		light.Delete();
	}
	
	[ClientRpc]
	public static void ShowMenu()
	{
		HUD.Instance.AddChild<SpookyJam2022.MainMenu>();
	}
}
