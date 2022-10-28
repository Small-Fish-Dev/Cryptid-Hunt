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
		Log.Error( "TODO: Remove every instance of NextBot" );
	}
}
