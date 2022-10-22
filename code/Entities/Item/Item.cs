namespace SpookyJam2022;

public partial class Item
{
	public Container Container { get; set; }

	public static Item FromResource( string resourceName )
	{
		if ( !ItemResource.All.TryGetValue( resourceName, out var resource ) ) 
			return null;

		return new Item
		{
			Resource = resource
		};
	}
}
