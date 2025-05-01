namespace CryptidHunt;

public partial class Player
{
	public Item[] Inventory { get; set; } = new Item[16];

	public bool Give( Item item )
	{
		if ( Inventory == null || !Inventory.Any( x => !x.IsValid() ) ) return false; // Invalid or full inventory

		var firstEmptySlot = Array.FindIndex( Inventory, x => !x.IsValid() );
		Inventory[firstEmptySlot] = item;

		item.WorldPosition = WorldPosition;
		item.GameObject.SetParent( GameObject );
		item.GameObject.Enabled = false;

		return true;
	}

	public bool Equip( Item item )
	{
		return true;
		/*var type = item
			?.Resource
			?.Interactable
			?.TargetType;

		var interactable = TypeLibrary.Create<BaseInteractable>( type );
		if ( interactable == null )
		{
			// failed ?? why
			return;
		}

		interactable.ActiveItem = item;
		pawn.ChangeHolding( interactable );
		Sound.FromScreen( "sounds/items/pickup.sound" );*/
	}
}
