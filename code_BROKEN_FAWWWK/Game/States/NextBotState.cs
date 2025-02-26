﻿namespace SpookyJam2022.States;

public class NextBotState : BaseState
{
	public static int Deaths { get; set; } = 0;
	public static NextBot Nextbot { get; set; }

	public override void Init()
	{
		CryptidHunt.Player = new NextBotPlayer();
		CryptidHunt.PlayerClient.Pawn = CryptidHunt.Player;
		CryptidHunt.Player.Respawn();
	}
	
	public override void CleanUp()
	{
		foreach ( var ent in Entity.All )
			if ( ent is NextBot or NextBotPlayer )
				ent.Delete();
	}

	[ConCmd.Server("debug_select_fear")]
	public static void SelectFear( string fear )
	{
		if ( Nextbot != null ) return;

		Nextbot = new NextBot();
		Nextbot.SetImage( To.Everyone, fear );
	}
	
	[Event("nextbot.player.dead")]
	public void OnPlayerDeath()
	{
		Deaths++;

		if ( Deaths == 3 )
			new Action( async () =>
			{
				await GameTask.Delay( 2500 );
				Sound.FromWorld( "sounds/misc/glass_break_intro.sound", PlayerSpawn.Initial.Transform.PointToWorld( new Vector3( -50f, 30f, 72f ) ) ).SetVolume( 6 );
				await GameTask.Delay( 3000 );
				CryptidHunt.State = new GameplayState();
			} ).Invoke();
	}
}
