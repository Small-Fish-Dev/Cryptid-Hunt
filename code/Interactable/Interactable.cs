namespace CryptidHunt;

public class Interactable : Component
{
	[Property]
	public virtual string InteractDescription { get; set; } = "Take";
	[Property]
	public virtual bool Locked { get; set; } = false;
	[Property]
	public Vector3 PromptOffset3D { get; set; }
	[Property]
	public Vector2 PromptOffset2D { get; set; }

	public virtual void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		//player.Inventory?.Insert( Item, Amount );
		Sound.Play( "pickup", WorldPosition );
	}

	public virtual void Use( Player player )
	{
		if ( !player.IsValid() ) return;
		//if ( player == null
		//	|| ActiveItem == null
		//	|| ActiveItem.Container != player.Inventory ) return;

		//var index = player.Inventory.Items.IndexOf( ActiveItem );
		player.AddCameraShake( 0.3f, 8f );
		//player.Inventory.Remove( index );
	}
}
