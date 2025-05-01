using System.Text.Json.Serialization;

namespace CryptidHunt;

public enum AmountType
{
	Float = 0,
	Integer = 1
}

public partial class Item : Interactable
{
	[Property]
	public float Amount { get; set; } = 1;

	[Property]
	[Category( "Visual" )]
	public string Title { get; set; }

	[Property]
	[Category( "Visual" )]
	public string Description { get; set; } = "Empty description.";

	[Property]
	[Category( "Visual" )]
	public Color Color { get; set; } = Color.White;


	[Property]
	[Category( "Amount" )]
	public float MaxAmount { get; set; } = 1;

	[Property]
	[Category( "Amount" )]
	public AmountType AmountType { get; set; }


	[Property]
	[Category( "Other" )] public float Weight { get; set; } = 1;

	[Hide, JsonIgnore] public Texture Icon { get; private set; }
	[Hide, JsonIgnore] public TypeDescription Interactable { get; private set; }
}
