namespace SpookyJam2022;

class NotePage : Panel
{

	public Label Text;
	public Panel HandPrint;

	public NotePage( string text, bool handPrint = false )
	{

		var page = AddChild<Panel>( "page" );

		if ( handPrint )
		{

			HandPrint = page.AddChild<Panel>( "handPrint" );

		}

		Text = page.AddChild<Panel>( "textContainer" )
			.AddChild<Label>( "text" );

		Text.SetText( text );

	}
	
	[Event( "CreateNotePage" )]
	public static void CreateNotePage( string text, bool handPrint = false )
	{

		NotePage page = new NotePage( text, handPrint );

		HUD.Instance.AddChild( page );

	}

	[Event( "HideNotePage" )]
	public void HideNotePage()
	{

		Delete();

	}

}
