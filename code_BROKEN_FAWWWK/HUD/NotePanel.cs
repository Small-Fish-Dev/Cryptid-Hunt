namespace SpookyJam2022;

class NotePanel : Panel
{

	public Label Text;
	public Panel HandPrint;

	public NotePanel( string text, bool handPrint = false )
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

		NotePanel page = new NotePanel( text, handPrint );

		HUD.Instance.AddChild( page );

	}

	[Event( "HideNotePage" )]
	public void HideNotePage()
	{

		Delete();

	}

}
