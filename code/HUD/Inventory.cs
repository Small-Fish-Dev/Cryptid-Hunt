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

	Panel itemContainer;

	public Inventory()
	{
		Instance = this;

		var titleContainer = AddChild<Panel>( "titleContainer" );
		var title = titleContainer.AddChild<Label>( "title" );
		title.Text = "BACKPACK";

		var weightBarContainer = titleContainer.AddChild<Panel>( "weightBarContainer" );
		var weightBar = weightBarContainer.AddChild<Panel>( "weightBar" );
		weightDisplay = weightBar.AddChild<Panel>( "inner" );

		var weightContainer = titleContainer.AddChild<Panel>( "weightContainer" );
		weightNumber = weightContainer.AddChild<Label>( "weight" );

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
	}
}
