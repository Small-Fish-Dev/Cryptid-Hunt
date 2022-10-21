namespace SpookyJam2022;

class NotePage : Panel
{

	public Label Text;

	public NotePage()
	{

		Text = AddChild<Panel>( "page" )
			.AddChild<Panel>( "textContainer" )
			.AddChild<Label>( "text" );

		Text.SetText( "Help!!!\nI am going to die here\nTHE MONSTER IS COMING\nIt comes from that damn Ape Tavern" );

	}

}
