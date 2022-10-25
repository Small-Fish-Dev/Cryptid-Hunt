namespace SpookyJam2022;

public partial class Container
{
	public enum Update
	{
		Initialize,
		Insert,
		Remove,
		Amount
	}

	public List<Client> UpdateTargets = new();

	private byte[] getAmountUpdate( Item item )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( ID );

				writer.Write( Items.IndexOf( item ) );
				writer.Write( item.Amount );
			}

			return stream.ToArray();
		}
	}

	private byte[] getInsertUpdate( Item item )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( ID );

				writer.Write( item.Index );
				writer.Write( item.Resource.ResourceName );
				writer.Write( item.Amount );
			}

			return stream.ToArray();
		}
	}

	private byte[] getInitialUpdate()
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( ID );

				writer.Write( Name );
				writer.Write( MaxWeight );
				// todo: send full item updates too
			}

			return stream.ToArray();
		}
	}

	public byte[] getRemoveUpdate( int index )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( ID );

				writer.Write( index );
			}

			return stream.ToArray();
		}
	}
}
