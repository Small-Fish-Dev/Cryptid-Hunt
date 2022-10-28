namespace SpookyJam2022.States;

public partial class CreditsMenuState : BaseState
{
	public override void Init()
	{

		ShowCredits( To.Everyone);

	}
	
	public override void CleanUp()
	{
		// The menu hides itself.
	}
	
	[ClientRpc]
	public static void ShowCredits()
	{

		HUD.Instance.SetTemplate( "/HUD/Credits.html" );
	}
}
