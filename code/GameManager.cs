namespace CryptidHunt;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }

	[Property]
	public SceneFile MainMenu { get; set; }

	[Property]
	public SoundEvent WindSound { get; set; }

	[Property]
	public GameObject EndGameRespawn { get; set; }

	[Property]
	public Interactable Computer { get; set; }

	[Property]
	public GameObject Screen { get; set; }

	[Property]
	public List<GameObject> EndingEnable { get; set; }
	[Property]
	public List<GameObject> EndingDisable { get; set; }

	[Property]
	public Credits CreditScreen { get; set; }

	SoundHandle _windSoundHandle;
	public bool ReadInitialNote { get; set; } = false;

	protected override void OnStart()
	{
		Instance = this;
	}

	TimeUntil _nextInsideCheck;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			PauseScreen.Paused = !PauseScreen.Paused;
			PauseScreen.Instance.Enabled = PauseScreen.Paused;
			Scene.TimeScale = PauseScreen.Paused ? 0f : 1f;
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( !_windSoundHandle.IsValid() || !_windSoundHandle.IsPlaying )
		{
			_windSoundHandle = Sound.Play( WindSound );
		}
		else
		{
			if ( !_nextInsideCheck ) return;
			_nextInsideCheck = 0.5f;

			var player = Player.Instance;
			if ( player.IsValid() )
			{
				var directions = new Vector3[]
				{
					Vector3.Zero,
					Vector3.Forward,
					Vector3.Backward,
					Vector3.Left,
					Vector3.Right
				};

				var isInside = true;

				foreach ( var direction in directions )
				{
					var offset = direction * 100f;
					var trace = Scene.Trace.Ray( player.WorldPosition + Vector3.Up * 10f, player.WorldPosition + offset + Vector3.Up * 10f )
						.IgnoreDynamic()
						.Run();

					var upTrace = Scene.Trace.Ray( trace.EndPosition, trace.EndPosition + Vector3.Up * 1500f )
						.IgnoreDynamic()
						.Run();

					if ( !upTrace.Hit )
					{
						isInside = false;
						break;
					}
				}

				var targetVolume = isInside ? 1f : 3f;
				_windSoundHandle.Volume = MathX.Lerp( _windSoundHandle.Volume, targetVolume, Time.Delta * 20f );
			}
		}
	}

	public void EndGame()
	{
		Player.Instance.WorldTransform = EndGameRespawn.WorldTransform;
		Player.Instance.Controller.EyeAngles = EndGameRespawn.WorldRotation;
		Player.Instance.ChangeHolding( null );
		Screen.Enabled = false;
		Computer.Enabled = false;

		var transferDoor = Scene.Components.Get<SceneTransferDoor>();
		if ( transferDoor.IsValid() )
			transferDoor.Enabled = false;

		foreach ( var obj in EndingEnable )
			obj.Enabled = true;

		foreach ( var obj in EndingDisable )
			obj.Enabled = false;
	}

	public async void Credits()
	{
		Sound.Play( "creepy_radio" );
		Sound.Play( "slam_flute" );

		await Task.DelaySeconds( 6f );

		GameUI.BlackScreen( 6f );
		await Task.DelaySeconds( 3f );
		Player.Instance.LockInputs = true;
		await Task.DelaySeconds( 3f );
		CreditScreen.Enabled = true;
	}

	[ConCmd( "end_game" )]
	public static void EndGameCommand()
	{
		GameManager.Instance.EndGame();
	}
}
