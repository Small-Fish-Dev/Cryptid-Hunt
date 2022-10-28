namespace SpookyJam2022;

class MainMenuButton : Panel
{

	public MainMenuButton()
	{

		AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			Game.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				//await GameTask.DelaySeconds( 2.5f );

				//Delete();

			} );

		} );

	}

}
