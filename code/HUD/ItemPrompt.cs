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

		if ( Local.Pawn is not Player player ) return;

		var interactable = player.FirstInteractable;

		SetClass( "hidden", interactable == null );

		if ( interactable == null ) return; 

		Prompt.SetText( interactable.UseDescription );

		var offsetPosition = interactable.Transform.PointToWorld( interactable.PromptOffset3D );
		var screenPosition = offsetPosition.ToScreen();

		container.Style.Top = Length.Fraction( screenPosition.y );
		container.Style.Left = Length.Fraction( screenPosition.x );

		var transform = new PanelTransform();
		transform.AddTranslateX( Length.Pixels( interactable.PromptOffset2D.x ) );
		transform.AddTranslateY( Length.Pixels( interactable.PromptOffset2D.y ) );

		container.Style.Transform = transform;

		Image.Style.SetBackgroundImage( interactable.Locked ? "ui/lock.png" : "ui/hand.png" );

	}

}
