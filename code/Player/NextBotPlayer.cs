namespace CryptidHunt;

public partial class NextBotPlayer : Component
{
	[Property]
	public PlayerController Controller { get; set; }

	[Property]
	public GameObject Camera { get; set; }

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !Controller.IsValid() ) return;
		if ( !Camera.IsValid() ) return;

		Camera.WorldRotation = Controller.EyeAngles;
	}
}
