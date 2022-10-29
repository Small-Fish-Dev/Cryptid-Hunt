namespace SpookyJam2022.States;

public class GameplayState : BaseState
{
	public override void Init()
	{
		var player = new Player();
		Game.Player = player;
		Game.PlayerClient.Pawn = player;
		player.Respawn();
		player.Inventory = new("Backpack", 30, target: Game.PlayerClient);
		
		Event.Run( "BeginGame" ); // glass break
	}

	public override void CleanUp()
	{
	}
}
