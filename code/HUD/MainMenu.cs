using SpookyJam2022.States;

namespace SpookyJam2022;

class MainMenu : Panel
{
	public static MainMenu Instance { get; private set; }

	Button startButton;
	Button creditsButton;
	Button quitButton;

	public MainMenu()
	{
		Instance = this;

		AddChild<Panel>( "Logo" );

		startButton = AddChild<Button>( "Button" );
		startButton.SetText( "Start Game" );
		startButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			Game.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				NetworkStart();

				Delete();

			} );

		} );

		creditsButton = AddChild<Button>( "Button" );
		creditsButton.SetText( "Credits" );
		creditsButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

		} );

		quitButton = AddChild<Button>( "Button" );
		quitButton.SetText( "Quit" );
		quitButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			Game.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				QuitGame();

			} );

		} );


	}

	[ConCmd.Server]
	private static void NetworkStart()
	{
		Game.State = new NextBotState();
	}

	[ConCmd.Server]
	private static void QuitGame()
	{

		var clients = Client.All.ToArray();

		foreach( var client in clients )
		{

			client.Kick();

		}

	}

}
