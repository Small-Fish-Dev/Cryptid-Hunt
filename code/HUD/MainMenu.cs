namespace SpookyJam2022;

class MainMenu : Panel
{

	Button startButton;
	Button creditsButton;
	Button quitButton;

	public MainMenu()
	{

		AddChild<Panel>( "Logo" );

		startButton = AddChild<Button>( "Button" );
		startButton.SetText( "Start Game" );
		startButton.AddEventListener( "onclick", () =>
		{

			Style.PointerEvents = PointerEvents.None;

			Game.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				Event.Run( "BeginGame" );
				NetworkStart();

				Delete();

			} );

		} );

		creditsButton = AddChild<Button>( "Button" );
		creditsButton.SetText( "Credits" );

		quitButton = AddChild<Button>( "Button" );
		quitButton.SetText( "Quit" );


	}

	[ConCmd.Server]
	public void NetworkStart()
	{

		Event.Run( "BeginGame" );

	}

}
