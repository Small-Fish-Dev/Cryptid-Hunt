namespace CryptidHunt;

public partial class Player
{
	public Item[] Items { get; set; } = new Item[16];

	public bool Give( Item item )
	{
		if ( Items == null || !Items.Any( x => !x.IsValid() ) ) return false; // Invalid or full inventory

		var firstEmptySlot = Array.FindIndex( Items, x => !x.IsValid() );
		Items[firstEmptySlot] = item;

		item.WorldPosition = WorldPosition;
		item.GameObject.SetParent( GameObject );
		item.GameObject.Enabled = false;

		return true;
	}

	public void Remove( Item item )
	{
		if ( Items == null ) return;

		var index = Array.FindIndex( Items, x => x == item );
		if ( index == -1 ) return;

		if ( Holding == item )
			ChangeHolding( null );

		item.GameObject.SetParent( null );
		item.GameObject.Enabled = true;
		Items[index] = null;
		Inventory.Instance.SelectedItem = null;
	}
}
