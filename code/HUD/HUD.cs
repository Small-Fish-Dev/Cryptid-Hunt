﻿namespace SpookyJam2022;

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

			Instance.Delete();

		}

		Instance = new HUD();

	}

	public HUD()
	{

		SetTemplate( "/HUD/Layout.html" );

	}

}