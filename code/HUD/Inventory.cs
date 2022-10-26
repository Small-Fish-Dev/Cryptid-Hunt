namespace SpookyJam2022;

public class Inventory : Panel
{
	public static Inventory Instance { get; private set; }

	private bool active = false;
	public bool Active { 
		get => active; 
		set
		{
			active = value;
			Style.Opacity = value ? 1f : 0f;
		}
	}

	Panel weightDisplay;
	Label weightNumber;
	Label weightOver;

	Panel itemContainer;

	public Inventory()
	{
		Instance = this;

		var container = AddChild<Panel>( "container" );

		var titleContainer = container.AddChild<Panel>( "titleContainer" );
		var title = titleContainer.AddChild<Label>( "title" );
		title.Text = "BACKPACK";

		var weightBarContainer = container.AddChild<Panel>( "weightBarContainer" );
		var weightBar = weightBarContainer.AddChild<Panel>( "weightBar" );
		weightDisplay = weightBar.AddChild<Panel>( "inner" );

		var weightContainer = container.AddChild<Panel>( "weightContainer" );
		weightNumber = weightContainer.AddChild<Label>( "weight" );
		weightOver = weightContainer.AddChild<Label>( "weight" );

		itemContainer = AddChild<Panel>( "itemContainer" );

		Refresh();
	}

	[Event.BuildInput]
	private void buildInput( InputBuilder input )
	{
		if ( input.Released( InputButton.Score ) )
			Active = !Active;
	}

	public void Refresh()
	{
		if ( Local.Pawn is not Player pawn ) return;

		var inventory = pawn.Inventory;
		if ( inventory == null ) return;

		weightNumber.Text = $"{inventory.Weight:N1} / {inventory.MaxWeight:N1} KG";

		var len = MathF.Min( inventory.Weight / inventory.MaxWeight, 1f );
		weightDisplay.Style.Width = Length.Fraction( len );
		weightDisplay.Style.BackgroundTint = len == 1f ? new Color( 0.8f, 0.4f, 0.4f ) : Color.White;

		weightOver.Text = $"({(inventory.MaxWeight - inventory.Weight):N1} KG)";
		weightOver.Style.FontColor = len == 1f ? new Color( 0.8f, 0.4f, 0.4f ) : new Color( 0.4f, 0.8f, 0.4f );
	}
}
