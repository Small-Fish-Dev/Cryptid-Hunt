namespace SpookyJam2022;

public partial class Player
{
	public Container Inventory { get; set; }

	/// <summary>
	/// Sends a container update to To targets.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="data"></param>
	[ClientRpc]
	public static void UpdateContainer( Container.Update type, byte[] data )
	{
		using ( var stream = new MemoryStream( data ) )
		{
			using ( var reader = new BinaryReader( stream ) )
			{
				var id = reader.ReadInt32();
				List<(int, int)> update = null;
				Container container = null;

				if ( type != Container.Update.Initialize
					&& !Container.All.TryGetValue( id, out container ) )
				{
					Log.Error( $"Failed to update non existing container ({id})." );
					return;
				}

				switch ( type )
				{
					case Container.Update.Initialize:
						var name = reader.ReadString();

						var width = reader.ReadInt32();
						var height = reader.ReadInt32();

						container = new( name, width, height, id );

						// todo: initialization might include items, read through them

						break;

					case Container.Update.Insert:
						var x = reader.ReadInt32();
						var y = reader.ReadInt32();

						var item = Item.FromResource( reader.ReadString() );
						var res = item.Resource;

						update = new();

						for ( int i = y; i < y + res.Height; i++ )
							for ( int j = x; j < x + res.Width; j++ )
							{
								container.Items[i, j] = item;
								update.Add( (j, i) );
							}

						// todo: read item data

						break;

					case Container.Update.Remove:
						var rx = reader.ReadInt32();
						var ry = reader.ReadInt32();
						var w = reader.ReadInt32();
						var h = reader.ReadInt32();

						update = new();

						for ( int i = ry; i < ry + h; i++ )
							for ( int j = rx; j < rx + w; j++ )
							{
								container.Items[i, j] = null;
								update.Add( (j, i) );
							}

						break;

					case Container.Update.Data:

						break;
				}

				if ( Local.Pawn is Player player && player.Inventory == null )
					player.Inventory = container;

				if ( ContainerDisplay.All.TryGetValue( container, out var display ) )
					display.Refresh( update );
			}
		}
	}
}
