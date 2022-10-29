using Sandbox;
using System.Numerics;

namespace SpookyJam2022.States;

public partial class MainMenuState : BaseState
{
	public override void Init()
	{
		NextBotState.Deaths = 0;
		
		ShowMenu(To.Everyone);

		foreach ( var camera in Entity.All.OfType<ScriptedEventCamera>() ) // Find camera (shitt)
		{
			if ( camera.Name.Contains( "MainMenu" ) )
			{

				foreach ( var ply in Entity.All.OfType<Player>() )
				{
					ply.OverrideCamera = camera;
					ply.ScriptedEvent = true;
					ply.LockInputs = true;

				}

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
