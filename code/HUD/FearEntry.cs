using SpookyJam2022.States;

namespace SpookyJam2022;

public class FearEntry : TextEntry
{
	public FearEntry()
	{
		CaretColor = Color.Black;
		Placeholder = "(type and press enter)";
		Multiline = true;
	}
	
	public override void OnButtonEvent( ButtonEvent e )
	{
		if ( !IsValid )
			return;
		
		if ( e.Button == "enter" ) // gary plizze maek butt oncodes public
		{
			NextBotState.SelectFear( Text );
			Parent?.Delete( true );
			return;
		}
	
		base.OnButtonEvent( e );
	}
}
