namespace SpookyJam2022;

public class ContainerDisplay : Panel
{
	private class Borders : Panel
	{
		private ContainerDisplay display;

		public Borders( ContainerDisplay display )
		{
			this.display = display;

			Style.ZIndex = 2;
			Style.Overflow = OverflowMode.Visible;
		}

		public override void DrawBackground( ref RenderState state )
		{
			if ( display == null )
			{
				Delete( true );
				return;
			}

			var left = display.Box.Left;
			var top = display.Box.Top;

			foreach ( var pair in display.grids )
			{
				var pos = pair.Key;
				var item = display.Container?.Items?[pos.y, pos.x];

				var posAdd = item != null ? 0 : 1;
				var sizeAdd = item != null ? 1 : -1;
				var size = new Vector2( item?.Resource.Width ?? 1, item?.Resource.Height ?? 1 );

				var rect = new Rect(
					left + pos.x * display.GridSize.x * ScaleToScreen + posAdd,
					top + pos.y * display.GridSize.y * ScaleToScreen + posAdd,
					size.x * display.GridSize.x * ScaleToScreen + sizeAdd,
					size.y * display.GridSize.y * ScaleToScreen + sizeAdd ).Floor();

				Extensions.DrawOutline( rect, item == null ? new Color( 0.7f, 0.7f, 0.7f ) : Color.Black, 1 );
			}

			var sizeRect = new Rect(
				left,
				top,
				display.Container.Width * display.GridSize.x * ScaleToScreen + 1,
				display.Container.Height * display.GridSize.y * ScaleToScreen + 1 ).Floor();

			Extensions.DrawOutline( sizeRect, Color.Black, 1 );
		}
	}

	public static Dictionary<Container, ContainerDisplay> All { get; } = new();

	public Container Container { get; set; }
	public Vector2 GridSize => 50f;

	private Dictionary<(int x, int y), Panel> grids = new();

	public ContainerDisplay( Container container )
	{
		Container = container;
		Refresh();
		AddChild( new Borders( this ) );

		if ( All.ContainsKey( container ) )
			All.Remove( container );
		
		All.Add( container, this );
	}

	~ContainerDisplay()
	{
		if ( All.ContainsKey( Container ) )
			All.Remove( Container );
	}

	private void refreshGrid( int x, int y )
	{
		var item = Container.Items[y, x];
		if ( item != null && (item.X != x || item.Y != y) )
			return;

		var grid = new Panel( this, "grid" );
		grid.Style.Width = GridSize.x * (item?.Resource.Width ?? 1);
		grid.Style.Height = GridSize.y * (item?.Resource.Height ?? 1);
		grid.Style.Left = (item?.X ?? x) * GridSize.x;
		grid.Style.Top = (item?.Y ?? y) * GridSize.y;
		
		var tint = grid.AddChild<Panel>( "tint" );
		tint.Style.BackgroundColor = item?.Resource.Color ?? Color.Transparent;

		grids.Add( (x, y), grid );
	}

	public void Refresh( List<(int x, int y)> targets = null )
	{
		if ( Container == null )
		{
			Delete( true );
			Log.Error( "Trying to update a container that doesn't exist." );

			return;
		}

		Style.Width = GridSize.x * Container.Width;
		Style.Height = GridSize.y * Container.Height;

		if ( targets != null )
		{
			foreach ( var grid in targets )
			{
				if ( grids.TryGetValue( (grid.x, grid.y), out var panel ) )
				{
					panel?.Delete( true );
					grids.Remove( (grid.x, grid.y) );
				}

				refreshGrid( grid.x, grid.y );
			}

			return;
		}

		for ( int x = 0; x < Container.Width; x++ )
			for ( int y = 0; y < Container.Height; y++ )
			{
				if ( grids.TryGetValue( (x, y), out var panel ) )
				{
					panel?.Delete( true );
					grids.Remove( (x, y) );
				}

				refreshGrid( x, y );
			}
	}
}
