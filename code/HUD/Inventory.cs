namespace SpookyJam2022;

public class Inventory : Panel
{
	private class ItemViewer : ScenePanel
	{
		SceneModel obj;
		Vector2 oldPos;
		bool shouldMove = false;

		float pitch;
		float yaw;

		public ItemViewer( Item item )
		{
			var world = new SceneWorld();
			var model = Model.Load( item.Resource.Model )
					?? Model.Load( "models/dev/error.vmdl" );
			obj = new SceneModel(
				world,
				model,
				new Transform( item.Resource.Position, item.Resource.Angles.ToRotation(), 1f ) );
			var light = new SceneLight( world, Vector3.Forward * 100f + Vector3.Up * 20f, 100f, Color.White * 0.5f );

			pitch = obj.Rotation.Pitch();
			yaw = obj.Rotation.Yaw();

			Camera.World = world;
			Camera.Position = Vector3.Forward * 100f;
			Camera.Rotation = Rotation.From( 0, 180, 0 );
			Camera.FieldOfView = 60f;

			AddEventListener( "onmousedown", () => shouldMove = true );
			AddEventListener( "onmouseup", () => shouldMove = false );
		}

		[Event.Frame]
		private void onFrame()
		{
			if ( !HasHovered )
				shouldMove = false;

			if ( shouldMove )
			{				
				var deltaPos = Mouse.Position - oldPos;
				pitch = (pitch + deltaPos.y) % 360;
				yaw = (yaw + deltaPos.x) % 360;

				obj.Rotation = Rotation.From( pitch, yaw, 0 );
			}

			oldPos = Mouse.Position;
		}
	}

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

	Dictionary<int, Panel> slots = new();
	Panel slotContainer;

	Panel viewContainer;

	Item _selected;
	Item selected 
	{ 
		get => _selected;
		set
		{
			_selected = value;

			viewContainer?.Delete( true );

			if ( value != null )
				createView( value );
		}
	}
	Panel selectedPanel;

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

		var itemContainer = AddChild<Panel>( "itemContainer" );
		slotContainer = itemContainer.AddChild<Panel>( "slotContainer" );

		Refresh();
	}

	[Event.BuildInput]
	private void buildInput( InputBuilder input )
	{
		if ( Local.Pawn is not Player pawn ) return;
		if ( input.Released( InputButton.Score ) )
			Active = !Active;
	}

	private void refreshSlot( Container container, int index )
	{
		Panel panel;
		if ( !slots.TryGetValue( index, out panel ) )
		{
			slots.Add( index, panel = new Panel( slotContainer, "slot" ) );
			panel.AddEventListener( "onclick", () =>
			{
				if ( container[index] == null )
				{
					selectedPanel?.SetClass( "selected", false );
					selected = null;

					return;
				}

				selectedPanel?.SetClass( "selected", false );
				selected = container[index];
				selectedPanel = panel;

				panel.SetClass( "selected", true );
			} );
		}
		panel.DeleteChildren( true );

		var item = container[index];
		if ( item != null )
		{
			var weight = panel.AddChild<Panel>( "weight" );
			weight.AddChild<Panel>( "icon" );
			var text = weight.AddChild<Label>( "text" );
			text.Text = $"{(item.Amount * item.Resource.Weight):N1} KG";

			if ( item.Resource.MaxAmount != 1 )
			{
				var amount = panel.AddChild<Panel>( "amount" );
				var amountText = amount.AddChild<Label>( "text" );
				amountText.Text = item.Resource.AmountType switch
				{
					Item.AmountType.Integer => $"{item.Amount:N0}/{item.Resource.MaxAmount:N0}",
					Item.AmountType.Float => $"{item.Amount:N1}/{item.Resource.MaxAmount:N1}",
					_ => "",
				};
			}

			var iconContainer = panel.AddChild<Panel>( "iconContainer" );
			var icon = iconContainer.AddChild<Panel>( "itemIcon" );
			icon.Style.BackgroundImage = item.Resource.Icon;
		}

		panel.SetClass( "hasItem", item != null );
		panel.SetClass( "selected", item == selected );

		if ( item == selected )
			selectedPanel = panel;
	}

	private void createView( Item item )
	{
		viewContainer = AddChild<Panel>( "viewContainer" );
		var titleContainer = viewContainer.AddChild<Panel>( "titleContainer" );
		titleContainer.AddChild<Label>( "title" ).Text = $"{item.Resource.Title}";
		titleContainer.AddChild<Label>( "description" ).Text = $"{item.Resource.Description}";

		var viewer = new ItemViewer( item );
		viewer.AddClass( "view" );
		viewContainer.AddChild( viewer );
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

		for ( int i = 0; i < 24; i++ )
			refreshSlot( inventory, i );
	}
}
