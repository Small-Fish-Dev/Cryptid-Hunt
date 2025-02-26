﻿using SpookyJam2022.States;

namespace SpookyJam2022;

class MainMenu : Panel
{
	public static MainMenu Instance { get; private set; }

	Button startButton;
	Button creditsButton;
	Button quitButton;
	Button afterGameButton;

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

			CryptidHunt.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				NetworkStartNextbot();

				Delete();

			} );

		} );
		/*

		startButton = AddChild<Button>( "Button" );
		startButton.SetText( "SKIP NEXTBOT" );
		startButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			Game.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				NetworkStartGame();

				Delete();

			} );

		} ); 
		
		afterGameButton = AddChild<Button>( "Button" );
		afterGameButton.SetText( "CUTSCENE" );
		afterGameButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			Game.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				NetworkCutscene();

				Delete();

			} );

		} );
		*/
		creditsButton = AddChild<Button>( "Button" );
		creditsButton.SetText( "Credits" );
		creditsButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			CryptidHunt.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				NetworkCredits();

				Delete();

			} );

		} );

		quitButton = AddChild<Button>( "Button" );
		quitButton.SetText( "Quit" );
		quitButton.AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Style.PointerEvents = PointerEvents.None;

			CryptidHunt.Instance.StartBlackScreen();

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 2.5f );

				QuitGame();

			} );

		} );


	}

	[ConCmd.Server]
	private static void NetworkCredits()
	{
		CryptidHunt.State = new CreditsMenuState();
	}

	[ConCmd.Server]
	private static void NetworkStartNextbot()
	{
		CryptidHunt.State = new NextBotState();
	}



	[ConCmd.Server]
	private static void NetworkStartGame()
	{
		CryptidHunt.State = new GameplayState();
	}

	[ConCmd.Server]
	private static void QuitGame()
	{

		var clients = Game.Clients.ToArray();

		foreach( var client in clients )
		{

			client.Kick();

		}

	}

	[ConCmd.Server]
	private static void NetworkCutscene()
	{

		CryptidHunt.State = new AfterGameState();

	}

}
