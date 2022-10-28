namespace SpookyJam2022.States;

public class NextBotState : BaseState
{
	public static int Deaths { get; set; } = 0;
	
	public override void Init()
	{
		Game.Player = new NextBotPlayer();
		Game.PlayerClient.Pawn = Game.Player;
		Game.Player.Respawn();
		
		Log.Error( "TODO: Show the fear choice" );
	}
	
	public override void CleanUp()
	{
		foreach ( var ent in Entity.All )
			if ( ent is NextBot || ent is NextBotPlayer )
				ent.Delete();
	}
	
	[Event("nextbot.player.dead")]
	public void OnPlayerDeath()
	{
		Deaths++;

		if ( Deaths == 3 )
			Game.State = new GameplayState();
	}
}
