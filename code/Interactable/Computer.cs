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
	public bool HadWindowBreak = false;
	public override string InteractDescription => Playing ? "Quit" : "Play";

	protected override void OnStart()
	{
		base.OnStart();

		StartGame();

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

	public async void StopGameSequence()
	{
		if ( HadWindowBreak )
		{
			return;
		}

		await Task.DelaySeconds( 25f );
		if ( Playing )
			SoundPoint.StartSound();
		HadWindowBreak = true;
		await Task.DelaySeconds( 2f );

		StopGame();
	}

	public void StopGame()
	{
		Playing = false;
		Camera.Enabled = false;
		Player.Instance.LockInputs = false;
		Light.Enabled = true;
		GameManager.Instance.EscapeOverride = null;
	}

	public void StartGame()
	{
		Playing = true;
		Camera.Enabled = true;
		Player.Instance.LockInputs = true;
		GameManager.Instance.EscapeOverride = () =>
			{
				if ( !Playing )
				{
					GameManager.Instance.EscapeOverride = null;
					return false;
				}

				// Don't let the player escape early
				if ( HadWindowBreak )
				{
					StopGame();
				}
				return true;
			};
	}

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		if ( Playing )
		{
			StopGame();
		}
		else
		{
			StartGame();
		}
	}

	[ConCmd( "skip_nextbot" )]
	public static void SkipNextbot()
	{
		var computer = Game.ActiveScene.Components.Get<Computer>( FindMode.EverythingInSelfAndDescendants );
		computer.StopGame();
		computer.Started = true;

		var screen = Game.ActiveScene.Components.Get<ComputerScreen>( FindMode.EverythingInSelfAndDescendants );
		screen.Input.Blur();
	}
}
