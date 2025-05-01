namespace CryptidHunt;

public class Interactable : Component
{
	[Property]
	public string UseDescription { get; set; } = "Take";
	[Property]
	public bool Locked { get; set; } = false;
	[Property]
	public Vector3 PromptOffset3D { get; set; }
	[Property]
	public Vector2 PromptOffset2D { get; set; }

	public virtual void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		//player.Inventory?.Insert( Item, Amount );
		Sound.Play( "pickup", WorldPosition );
		Destroy();
	}

	public virtual void Use( Player player )
	{
		if ( !player.IsValid() ) return;
		//if ( player == null
		//	|| ActiveItem == null
		//	|| ActiveItem.Container != player.Inventory ) return;

		//var index = player.Inventory.Items.IndexOf( ActiveItem );
		player.AddCameraShake( 0.5f, 10f );
		//player.Inventory.Remove( index );
	}
}
