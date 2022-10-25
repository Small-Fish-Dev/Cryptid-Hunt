namespace SpookyJam2022;

class HUD : RootPanel
{

	public static HUD Instance { get; set; }

	[Event.Hotload] // For when you don't want the main menu to pop up
	private static void reloadHUD()
	{

		if ( Host.IsServer ) return;

		if ( Instance != null )
		{

			Instance.Delete( true );

		}

		Instance = new HUD();
		Instance.ChildrenOfType<MainMenu>().FirstOrDefault().Delete();

	}


	[Event( "GameStart" )]
	private static void createHUD()
	{

		if ( Host.IsServer ) return;

		if ( Instance != null )
		{

			Instance.Delete( true );

		}

		Instance = new HUD();

	}

	public HUD()
	{

		SetTemplate( "/HUD/Layout.html" );

		/*try
		{
			AddChild( new ContainerDisplay( (Local.Pawn as Player).Inventory ) );
		}
		catch { }*/
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

}
