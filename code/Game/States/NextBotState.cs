namespace SpookyJam2022.States;

public class NextBotState : BaseState
{
	public static int Deaths { get; set; } = 0;
	public static NextBot Nextbot { get; set; }

	public override void Init()
	{
		Game.Player = new NextBotPlayer();
		Game.PlayerClient.Pawn = Game.Player;
		Game.Player.Respawn();
	}
	
	public override void CleanUp()
	{
		foreach ( var ent in Entity.All )
			if ( ent is NextBot || ent is NextBotPlayer )
				ent.Delete();
	}

	[ConCmd.Server]
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
				Sound.FromWorld( "sounds/misc/glass_break_intro.sound", PlayerSpawn.Initial.Transform.PointToWorld( new Vector3( -50f, 30f, 72f ) ) ).SetVolume( 10 );
				await GameTask.Delay( 3000 );
				Game.State = new GameplayState();
			} ).Invoke();
	}
}
