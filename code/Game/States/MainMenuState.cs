namespace SpookyJam2022.States;

public partial class MainMenuState : BaseState
{
	public override void Init()
	{
		NextBotState.Deaths = 0;
		
		ShowMenu(To.Everyone);
	}
	
	public override void CleanUp()
	{
		// The menu hides itself.
	}
	
	[ClientRpc]
	public static void ShowMenu()
	{
		HUD.Instance.AddChild<SpookyJam2022.MainMenu>();
	}
}
