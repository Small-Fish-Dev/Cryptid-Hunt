namespace SpookyJam2022;

class HUD : RootPanel
{
	public static HUD Instance { get; set; }

	public HUD()
	{
		StyleSheet.Load( "code/HUD/Style.scss" );
		
		Instance = this;
	}

	[Event.Hotload] // For when you don't want the main menu to pop up
	private static void reloadHUD()
	{

		if ( Game.IsServer ) return;

		Instance?.Delete( true );

		Instance = new HUD();
		//Instance.ChildrenOfType<MainMenu>().FirstOrDefault().Delete();

	}

	[Event("BlackScreen")]
	public void AddBlackScreen()
	{

		AddChild( new BlackScreen() );

	}

	[Event( "ShowArea" )]
	public void AddZoneHint( string Name )
	{

		AddChild( new ZoneHint( Name ));

	}

	[Event( "InputHint" )]
	public void AddInputHint( string buttonHint, string hint )
	{

		string button = Input.GetKeyWithBinding( buttonHint ).ToUpper();
		string text = $"Press [{button}] {hint}";

		AddChild( new ZoneHint( text, text.Length / 20f ) );

	}

}
