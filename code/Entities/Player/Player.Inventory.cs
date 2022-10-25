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
						var maxWeight = reader.ReadSingle();

						container = new( name, maxWeight, id );

						// todo: initialization might include items, read through them

						break;

					case Container.Update.Insert:
						var index = reader.ReadInt32();

						var item = Item.FromResource( reader.ReadString() );
						item.Index = index;
						item.Container = container;

						var amount = reader.ReadSingle();
						item.Amount = amount;

						container.Items.Insert( index, item );
						container.Weight += amount * item.Resource.Weight;

						break;

					case Container.Update.Remove:
						var rindex = reader.ReadInt32();
						var ritem = container.Items.ElementAtOrDefault( rindex );
						if ( ritem == null )
						{
							Log.Error( "Trying to remove non-existing item on client." );
							return;
						}

						container.Weight -= ritem.Amount * ritem.Resource.Weight;
						container.Items.RemoveAt( rindex );

						break;

					case Container.Update.Amount:
						var aindex = reader.ReadInt32();
						var aamount = reader.ReadSingle();

						var aitem = container.Items.ElementAtOrDefault( aindex );
						if ( aitem == null )
						{
							Log.Error( "Trying to change the amount of a non-existing item on client." );
							return;
						}

						container.Weight += (aamount - aitem.Amount) * aitem.Resource.Weight;
						aitem.Amount = aamount;

						break;
				}

				if ( Local.Pawn is Player player && player.Inventory == null )
					player.Inventory = container;

				Log.Error( container.ToString() );
			}
		}
	}
}
