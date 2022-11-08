namespace SpookyJam2022.States;

public partial class GameplayState : BaseState
{
	public override void Init()
	{
		NetworkGameUI(To.Everyone);
		
		var player = new Player();
		Game.Player = player;
		Game.PlayerClient.Pawn = player;
		player.Respawn();
		player.Inventory = new( "Backpack", 20, target: Game.PlayerClient );

		Event.Run( "BeginGame" ); // glass break
	}

	public override void CleanUp()
	{
	}

	[ClientRpc]
	public static void NetworkGameUI()
	{
		HUD.Instance.SetTemplate( "/HUD/GameUI.html" );
	}
}
