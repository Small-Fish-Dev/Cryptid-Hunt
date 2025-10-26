namespace CryptidHunt;

public class Settings
{
	private const string SETTINGS_FILE = "settings.json";

	public static Settings Instance
	{
		get
		{
			_instance ??= new Settings();
			return _instance;
		}
		private set
		{
			_instance = value;
		}
	}
	private static Settings _instance;

	public class SettingsSchema
	{
		public bool BeatenTheGame { get; set; }
		public float? Darkness { get; set; }
	}

	public bool BeatenTheGame => _settings.BeatenTheGame;
	public float? Darkness => _settings.Darkness;

	public SettingsSchema DefaultSettings => new() { BeatenTheGame = false, Darkness = null };

	private SettingsSchema _settings;

	public Settings()
	{
		try
		{
			_settings = FileSystem.Data.ReadJson( SETTINGS_FILE, DefaultSettings );
			Validate();
		}
		catch ( Exception ex )
		{
			Log.Error( $"Caught an exception while trying to load the settings, falling back to defaults: ({ex})" );
			_settings = DefaultSettings;
		}
	}

	private void Validate()
	{
		_settings.Darkness = Darkness?.Clamp( 0f, 1f );
	}

	public void Change( Action<SettingsSchema> settingsChanger )
	{
		settingsChanger?.Invoke( _settings );
		Save();
	}

	public void Save()
	{
		FileSystem.Data.WriteJson( SETTINGS_FILE, _settings );
	}

	public void Reset()
	{
		_settings = DefaultSettings;
		Save();
	}

	[ConCmd( "debug_beat_the_game" )]
	static void DebugBeatTheGame()
	{
		Settings.Instance.Change( settings => settings.BeatenTheGame = true );
	}
}
