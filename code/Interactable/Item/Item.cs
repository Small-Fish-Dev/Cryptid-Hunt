using System.Text.Json.Serialization;

namespace CryptidHunt;

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
	public Texture Icon { get; set; }

	[Property]
	[Category( "Visual" )]
	public ModelRenderer Model { get; set; }

	[Property]
	[Category( "Other" )]
	public int MaxAmount { get; set; } = 1;

	[Property]
	[Category( "Other" )]
	public float Weight { get; set; } = 1;
}
