using Sandbox.UI;
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
	[ImageAssetPath]
	public string Icon { get; set; }

	[Property]
	[Category( "Visual" )]
	public ModelRenderer Model { get; set; }

	[Property]
	[Category( "Other" )]
	public int MaxAmount { get; set; } = 1;

	[Property]
	[Category( "Other" )]
	public float Weight { get; set; } = 1;

	[Property]
	[Category( "Other" )]
	public bool Useable { get; set; } = false;

	[Property]
	[Category( "Other" )]
	public string UseDescription { get; set; } = "Equip";

	[Property]
	[Category( "Other" )]
	public Vector3 ViewModelOffset { get; set; }

	[Property]
	[Category( "Other" )]
	public Rotation ViewModelRotation { get; set; }

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		if ( player.Give( this ) )
			Sound.Play( "pickup", WorldPosition );
	}

	public override void Use( Player player )
	{
		base.Use( player );
		player.ChangeHolding( this );
	}

	public virtual void Attack( Player player )
	{
		if ( !player.IsValid() ) return;
	}
}
