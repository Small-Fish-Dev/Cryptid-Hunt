using Sandbox;
using System.Numerics;

namespace CryptidHunt;

public partial class Computer : Interactable
{
	[Property]
	public GameObject Light { get; set; }

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
		Player.Instance.LockInputs = true;

		MusicHanlder = Sound.Play( Music );
	}

	protected override void OnFixedUpdate()
	{
		if ( Fear != null )
		{
			NextBot.SpriteRenderer.Sprite = Sprite.FromTexture( Fear );
		}

		MusicHanlder.Volume = Playing ? 0.2f : 0f;
	}

	public async void StopGame()
	{
		await Task.DelaySeconds( 25f );
		if ( Playing )
			SoundPoint.StartSound();
		await Task.DelaySeconds( 2f );

		if ( Playing )
		{
			Playing = false;
			Camera.Enabled = false;
			Player.Instance.LockInputs = false;
			Light.Enabled = true;
		}
	}

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		Playing = !Playing;
		Camera.Enabled = Playing;
		Player.Instance.LockInputs = Playing;
	}

	[ConCmd( "skip_nextbot" )]
	public static void SkipNextbot()
	{
		var computer = Game.ActiveScene.Components.Get<Computer>( FindMode.EverythingInSelfAndDescendants );
		computer.Playing = false;
		computer.Started = true;
		computer.Camera.Enabled = false;
		computer.Light.Enabled = true;
		Player.Instance.LockInputs = false;

		var screen = Game.ActiveScene.Components.Get<ComputerScreen>( FindMode.EverythingInSelfAndDescendants );
		screen.Started = true;
		screen.Input.Blur();
	}
}
