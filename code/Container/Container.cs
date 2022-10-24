namespace SpookyJam2022;

public partial class Container
{
	public enum Update
	{
		Initialize,
		Insert,
		Remove,
		Data
	}

	private static Dictionary<int, Container> all = new();
	public static IReadOnlyDictionary<int, Container> All => all;

	public string Name { get; private set; }
	public int ID { get; private set; }
	public int Width { get; private set; }
	public int Height { get; private set; }

	public Item[,] Items { get; private set; }

	public List<Client> UpdateTargets = new();

	public Container( string name, int width, int height, int? id = null, Client target = null )
	{
		ID = id ?? all.Count;

		Name = name;
		Width = width;
		Height = height;

		Items = new Item[height, width];
		
		all.Add( ID, this );

		if ( target != null )
			UpdateTargets.Add( target );

		if ( Host.IsServer )
		{
			using ( var stream = new MemoryStream() )
			{
				using ( var writer = new BinaryWriter( stream ) )
				{
					writer.Write( ID );

					writer.Write( Name );
					writer.Write( Width );
					writer.Write( Height );
				}

				Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Initialize, stream.ToArray() );
			}
		}
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
		for ( int i = 0; i < Height; i++ )
			for ( int j = 0; j < Width; j++ )
			{
				if ( isFree( j, i, w, h ) )
					return (j, i);
			}

		return null;
	}

	/// <summary>
	/// Looks for a item of specific type.
	/// </summary>
	/// <param name="resourceName"></param>
	/// <returns></returns>
	public (int x, int y)? Find( string resourceName )
	{
		for ( int i = 0; i < Height; i++ )
			for ( int j = 0; j < Width; j++ )
			{
				var item = Items[i, j];
				if ( item != null && item.Resource?.ResourceName == resourceName )
					return (j, i);
			}

		return null;
	}

	/// <summary>
	/// Adds a item to a container.
	/// </summary>
	/// <returns>A boolean telling you if the item was succesfully added.</returns>
	public bool Insert( Item item, Item.ItemData data = null )
	{
		Host.AssertServer();

		var res = item.Resource;
		var freeSpot = findFreeSpot( res.Width, res.Height );
		if ( freeSpot == null ) 
			return false;

		var x = freeSpot.Value.x;
		var y = freeSpot.Value.y;
		setItem( x, y, item );
		if ( data != null )
			item.OverrideData( data );

		item.Container = this;
		item.X = x;
		item.Y = y;

		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( ID );

				writer.Write( x );
				writer.Write( y );

				writer.Write( res.ResourceName );

				// todo: write item data
			}

			Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Insert, stream.ToArray() );
		}

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
		Host.AssertServer();

		var item = Items[y, x];
		if ( item == null ) return false;
		item.Container = null;

		var res = item.Resource;
		for ( int i = y; i < y + res.Height; i++ )
			for ( int j = x; j < x + res.Width; j++ )
				Items[i, j] = null;

		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( ID );

				writer.Write( x );
				writer.Write( y );
				writer.Write( res.Width );
				writer.Write( res.Height );
			}

			Player.UpdateContainer( To.Multiple( UpdateTargets ), Update.Remove, stream.ToArray() );
		}

		return true;
	}

	public override string ToString()
	{
		var result = "";
		for ( int i = 0; i < Height; i++ )
		{
			for ( int j = 0; j < Width; j++ )
			{
				var itemString = Items[i, j] == null ? "-" : Items[i, j].Resource.ResourceName.Substring( 0, 1 );
				result += $"{itemString}\t";
			}

			result += "\n";
		}

		return result;
	}
}
