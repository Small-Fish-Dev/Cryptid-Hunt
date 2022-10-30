namespace SpookyJam2022;

class ItemPrompt : Panel
{

	Panel container;
	public Panel Image;
	public Label Prompt;

	public ItemPrompt()
	{

		container = AddChild<Panel>( "container" );

		Image = container.AddChild<Panel>( "image" );

		Prompt = container.AddChild<Panel>( "promptContainer" )
			.AddChild<Label>( "prompt" );

	}

	[Event.Frame]
	void computePrompt()
	{
		SetClass( "hidden", true );

		if ( Local.Pawn is not Player player ) return;

		var interactable = player.FirstInteractable;

		SetClass( "hidden", interactable == null );

		if ( interactable == null ) return; 

		Prompt.SetText( interactable.UseDescription );

		var screenPosition = interactable.CollisionWorldSpaceCenter.ToScreen();
		container.Style.Top = Length.Fraction( screenPosition.y );
		container.Style.Left = Length.Fraction( screenPosition.x );

		Image.Style.SetBackgroundImage( interactable.Locked ? "ui/lock.png" : "ui/hand.png" );
	}

}
