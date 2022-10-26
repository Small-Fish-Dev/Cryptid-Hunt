namespace SpookyJam2022;

public partial class Item
{
	public enum AmountType
	{
		Liters,
		Kilograms,
		Amount
	}

	[GameResource( "Item", "item", "An item resource." )]
	public class ItemResource : GameResource
	{
		private static Dictionary<string, ItemResource> all = new();
		public static IReadOnlyDictionary<string, ItemResource> All => all;

		[Category( "Visual" )] public string Title { get; set; }
		[Category( "Visual" )] public string Description { get; set; } = "Empty description.";
		[Category( "Visual" )] public Color Color { get; set; } = Color.White;

		[Category( "Icon" )] public Angles Angles { get; set; }
		[Category( "Icon" )] public Vector3 Position { get; set; }

		[Category( "Amount" )] public float MaxAmount { get; set; } = 1;
		[Category( "Amount" )] public AmountType AmountType { get; set; } = AmountType.Amount;

		[Category( "Other" )] public float Weight { get; set; } = 1;

		protected override void PostLoad()
		{
			if ( !all.ContainsKey( ResourceName ) )
				all.Add( ResourceName, this );
		}
	}

	public ItemResource Resource { get; protected set; }
}
