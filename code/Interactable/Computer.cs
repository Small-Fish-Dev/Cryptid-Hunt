using Sandbox;

namespace CryptidHunt;

public partial class Computer : Interactable
{
	[Property]
	public GameObject Camera { get; set; }
	public bool Playing { get; set; } = false;
	public override string InteractDescription => Playing ? "Quit" : "Play";

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		Playing = !Playing;
		Camera.Enabled = Playing;
		player.LockInputs = Playing;
	}

}
