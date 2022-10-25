namespace SpookyJam2022;

public partial class Item
{
	[GameResource( "Item", "item", "An item resource." )]
	public class ItemResource : GameResource
	{
		private static Dictionary<string, ItemResource> all = new();
		public static IReadOnlyDictionary<string, ItemResource> All => all;

		public string Title { get; set; }
		public string Description { get; set; } = "Empty description.";

		public float Weight { get; set; } = 1;
		public float MaxAmount { get; set; } = 1;
		public Color Color { get; set; } = Color.White;

		protected override void PostLoad()
		{
			if ( !all.ContainsKey( ResourceName ) )
				all.Add( ResourceName, this );
		}
	}

	public ItemResource Resource { get; protected set; }
}
