using Sandbox;
using System.Numerics;

namespace CryptidHunt;

public partial class Computer : Interactable
{
	[Property]
	public GameObject Camera { get; set; }
	[Property]
	public SoundPointComponent SoundPoint { get; set; }
	public Texture Fear { get; set; }
	[Property]
	public NextBot NextBot { get; set; }
	[Property]
	public SoundEvent Music { get; set; }
	public SoundHandle MusicHanlder { get; set; }

	public bool Playing { get; set; } = true;
	public bool Started = false;
	public override string InteractDescription => Playing ? "Quit" : "Play";

	protected override void OnStart()
	{
		base.OnStart();
		Camera.Enabled = true;

		MusicHanlder = Sound.Play( Music );
	}

	protected override void OnFixedUpdate()
	{
		if ( Fear != null )
		{
			NextBot.SpriteRenderer.Texture = Fear;
		}

		MusicHanlder.Volume = Playing ? 0.2f : 0f;
	}

	public async void StopGame()
	{
		await Task.DelaySeconds( 40f );
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
