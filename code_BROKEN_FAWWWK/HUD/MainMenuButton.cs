using SpookyJam2022.States;
namespace SpookyJam2022;

class MainMenuButton : Panel
{

	public MainMenuButton()
	{

		AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			CryptidHunt.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				NetworkMainMenu();

				HUD.Instance.Children.Where( x => x.ElementName == "credits" )
				.FirstOrDefault()
				.Delete();

			} );

		} );

	}

	[ConCmd.Server]
	private static void NetworkMainMenu()
	{
		CryptidHunt.State = new MainMenuState();
	}

}
