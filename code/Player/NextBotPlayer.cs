namespace CryptidHunt;

public partial class NextBotPlayer : Component
{
	[Property]
	public Computer Computer { get; set; }

	[Property]
	public PlayerController Controller { get; set; }

	[Property]
	public GameObject Camera { get; set; }

	public bool Alive { get; set; } = true;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !Controller.IsValid() ) return;
		if ( !Camera.IsValid() ) return;

		if ( Computer.Playing && Computer.Started )
		{
			Controller.UseInputControls = true;
			Camera.WorldRotation = Controller.EyeAngles;
		}
		else
		{
			Controller.UseInputControls = false;
			Controller.WishVelocity = 0f;
		}
	}

	public void Die()
	{
		if ( Alive ) return;
		Alive = false;
	}

	public void Respawn()
	{
		Alive = true;
	}
}
