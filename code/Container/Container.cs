namespace SpookyJam2022;

public partial class Container
{
	private static Dictionary<int, Container> all = new();
	public static IReadOnlyDictionary<int, Container> All => all;

	public string Name { get; private set; }
	public int ID { get; private set; }
	public int Width { get; private set; }
	public int Height { get; private set; }

	public Item[,] Items { get; private set; }

	public Container( int width, int height, int? id = null, Client target = null )
	{
		ID = id ?? all.Count;

		Width = width;
		Height = height;

		Items = new Item[height, width];
		
		all.Add( ID, this );

		if ( target != null )
			UpdateTargets.Add( target );
	}

	private void setItem( int x, int y, Item item )
	{
		var res = item.Resource;
		for ( int i = y; i < y + res.Height; i++ )
			for ( int j = x; j < x + res.Width; j++ )
				Items[i, j] = item;
	}

	private bool isFree( int x, int y, int w, int h )
	{
		for ( int i = y; i < y + h; i++ )
			for ( int j = x; j < x + w; j++ )
			{
				if ( i < 0 || j < 0 
					|| i >= Height || j >= Width
					|| Items[i, j] != null ) return false;
			}

		return true;
	}

	private (int x, int y)? findFreeSpot( int w, int h )
	{
		for ( int j = 0; j < Width; j++ )
			for ( int i = 0; i < Height; i++ )
			{
				if ( isFree( j, i, w, h ) )
					return (j, i);
			}

		return null;
	}

	/// <summary>
	/// Adds a item to a container.
	/// </summary>
	/// <returns>A boolean telling you if the item was succesfully added.</returns>
	public bool Append( Item item )
	{
		var res = item.Resource;
		var freeSpot = findFreeSpot( res.Width, res.Height );
		if ( freeSpot == null ) 
			return false;

		setItem( freeSpot.Value.x, freeSpot.Value.y, item );
		return true;
	}

	/// <summary>
	/// Removes a item from some position.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns>A boolean telling you if the item was successfully removed.</returns>
	public bool Remove( int x, int y )
	{
		var item = Items[y, x];
		if ( item == null ) return false;

		var res = item.Resource;
		for ( int i = y; i < y + res.Height; i++ )
			for ( int j = x; j < x + res.Width; j++ )
				Items[i, j] = null;

		return true;
	}

	public override string ToString()
	{
		var result = "";
		for ( int j = 0; j < Height; j++ )
		{
			for ( int i = 0; i < Width; i++ )
			{
				var itemString = Items[i, j] == null ? "-" : Items[i, j].Resource.ResourceName.Substring( 0, 1 );
				result += $"{itemString}\t";
			}

			result += "\n";
		}

		return result;
	}
}
