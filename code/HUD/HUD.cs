namespace SpookyJam2022;

class HUD : RootPanel
{

	public static HUD Instance { get; set; }

	[Event.Hotload]
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

}
