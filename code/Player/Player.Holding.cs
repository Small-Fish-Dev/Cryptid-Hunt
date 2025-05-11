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
		{
			item.GameObject.SetParent( Camera.GameObject );
			item.LocalPosition = item.ViewModelOffset;
			item.LocalRotation = item.ViewModelRotation;
			item.GameObject.Enabled = true;

			foreach ( var collider in item.Components.GetAll<BoxCollider>( FindMode.EverythingInSelfAndDescendants ) )
				collider.Enabled = false;
		}

		Holding = item;
	}
}
