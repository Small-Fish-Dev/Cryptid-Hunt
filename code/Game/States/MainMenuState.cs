using Sandbox;
using System.Numerics;

namespace SpookyJam2022.States;

public partial class MainMenuState : BaseState
{
	public override void Init()
	{
		NextBotState.Deaths = 0;
		
		ShowMenu(To.Everyone);

		var cameras = Entity.All.OfType<ScriptedEventCamera>(); // collection was modified error waaa waa

		foreach ( var camera in cameras ) // Find camera (shitt)
		{
			if ( camera.Name.Contains( "MainMenu" ) )
			{

				foreach ( var ply in Entity.All.OfType<Player>() )
				{
					ply.OverrideCamera = camera;
					ply.ScriptedEvent = true;
					ply.LockInputs = true;

				}

				// TODO REMOVE!!
				var light = new PointLightEntity();
				light.Position = camera.Position;
				light.SetLightBrightness( 3f );
				light.SetLightColor( new Color( 1f, 0.95f, 0.8f ) );
				light.Range = 2500;

			}

		}
	}
	
	public override void CleanUp()
	{
		// The menu hides itself.
	}
	
	[ClientRpc]
	public static void ShowMenu()
	{
		HUD.Instance.AddChild<SpookyJam2022.MainMenu>();
	}
}
