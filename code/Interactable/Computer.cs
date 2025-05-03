using Sandbox;
using System.Numerics;

namespace CryptidHunt;

public partial class Computer : Interactable
{
	[Property]
	public GameObject Camera { get; set; }
	[Property]
	public SoundPointComponent SoundPoint { get; set; }

	public bool Playing { get; set; } = true;
	public bool Started = false;
	public override string InteractDescription => Playing ? "Quit" : "Play";

	protected override void OnStart()
	{
		base.OnStart();
		StopGame();
		Camera.Enabled = true;
	}

	public async void StopGame()
	{
		await Task.DelaySeconds( 300f ); // TODO MAKE 30
		SoundPoint.StartSound();
		await Task.DelaySeconds( 2f );
		Playing = false;
		Camera.Enabled = false;
	}

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		Playing = !Playing;
		Camera.Enabled = Playing;
		Player.Instance.LockInputs = Playing;
	}

}
