namespace CryptidHunt;

public partial class Player
{
	public Item Holding { get; set; }

	public void ChangeHolding( Item item )
	{
		if ( Holding.IsValid() )
		{
			Holding.GameObject.SetParent( GameObject );
			Holding.GameObject.Enabled = false;
		}

		if ( item.IsValid() ) // We can change holding to null
			item.GameObject.Enabled = true;

		Holding = item;
	}
}
