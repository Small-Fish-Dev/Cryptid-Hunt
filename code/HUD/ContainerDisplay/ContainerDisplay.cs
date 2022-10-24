namespace SpookyJam2022;

public class ContainerDisplay : Panel
{
	public static Dictionary<Container, ContainerDisplay> All { get; } = new();

	public Container Container { get; set; }
	public Vector2 GridSize => 50f;

	private Dictionary<(int x, int y), Panel> grids = new();

	public ContainerDisplay( Container container )
	{
		Container = container;
		Refresh();

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
