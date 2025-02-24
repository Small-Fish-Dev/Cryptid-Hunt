namespace SpookyJam2022;

public partial class Item
{
	public Container Container { get; set; }
	public int Index { get; set; }
	public float Amount { get; set; } = 1f;

	private Item() { }

	public static Item FromResource( string resourceName)
	{
		if ( !ItemResource.All.TryGetValue( resourceName, out var resource ) ) 
			return null;

		var item = new Item 
		{ 
			Resource = resource 
		};

		return item;
	}
}
