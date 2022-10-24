namespace SpookyJam2022;

public partial class Item
{
	public Container Container { get; set; }
	public int X { get; set; }
	public int Y { get; set; }

	private Item() { }

	public static Item FromResource( string resourceName, ItemData data = null )
	{
		if ( !ItemResource.All.TryGetValue( resourceName, out var resource ) ) 
			return null;

		var item = new Item { Resource = resource };
		if ( data != null ) 
			item.OverrideData( data );

		return item;
	}
}
