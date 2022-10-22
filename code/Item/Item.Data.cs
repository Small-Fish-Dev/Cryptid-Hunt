namespace SpookyJam2022;

public partial class Item
{
	public class ItemData
	{
		private Dictionary<string, object> data = new();
		private Item item;

		public ItemData( Item item )
		{
			this.item = item;
		}

		/// <summary>
		/// Sets ItemData to anything by key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		public void Set<T>( string key, T obj )
		{
			if ( Host.IsServer 
				&& item != null 
				&& item.Container != null )
			{
				// need to check if T can actually be written by binarywriter
			}

			if ( data.ContainsKey( key ) )
			{
				data[key] = obj;
				return;
			}

			data.Add( key, obj );
		}

		/// <summary>
		/// Tries to get ItemData by key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T Read<T>( string key )
		{
			if ( data.TryGetValue( key, out var obj ) )
				return (T)obj;

			return default(T);
		}
	}

	private ItemData data = null;
	public ItemData Data
	{
		get
		{
			if ( data == null )
				data = new( this );

			return data;
		}
	}

	/// <summary>
	/// Overrides item data.
	/// </summary>
	/// <param name="data"></param>
	public void OverrideData( ItemData data )
	{
		this.data = data;
	}
}
