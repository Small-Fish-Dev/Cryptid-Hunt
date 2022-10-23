namespace SpookyJam2022;

public class ContainerDisplay : Panel
{
	private static Dictionary<Container, ContainerDisplay> all = new();
	public static IReadOnlyDictionary<Container, ContainerDisplay> All => all;

	public Container Container { get; set; }
	public Vector2 GridSize => 50f;

	private Dictionary<(int x, int y), Panel> grids = new();

	public ContainerDisplay( Container container )
	{
		Container = container;
		Refresh();
	}

	private void refreshGrid( int x, int y )
	{
		Panel grid;
		if ( !grids.TryGetValue( (x, y), out grid ) )
		{
			AddChild( grid = new Panel() );
			grid.SetClass( "grid", true );
			grids.Add( (x, y), grid );
		}

		var item = Container.Items[y, x];
		grid.Style.Width = GridSize.x * (item?.Resource.Width ?? 1);
		grid.Style.Width = GridSize.y * (item?.Resource.Height ?? 1);
	}

	public void Refresh( List<(int, int)> targets = null )
	{
		if ( Container == null )
		{
			Delete( true );
			Log.Error( "Trying to update a container that doesn't exist." );

			return;
		}

		Style.Width = GridSize.x * Container.Width;
		Style.Height = GridSize.y * Container.Height;
	}
}
