namespace CryptidHunt;

public partial class Player
{
	public Item[] Inventory { get; set; } = new Item[16];

	public void Equip( int index )
	{
		var item = Inventory[index];
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
