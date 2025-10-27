namespace CryptidHunt;

public partial class Player
{
	public Item Holding { get; set; }

	public void ChangeHolding( Item item, bool byForce = false )
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
		else if ( byForce )
		{
			Sound.Play( "backpack_drop" );
			if ( Holding.IsValid() && Holding.DropSound.IsValid() )
			{
				var dropSound = Sound.PlayFile( Holding.DropSound );
				dropSound.Position = GameObject.WorldPosition
					+ Vector3.Down * 20f
					+ Random.Shared.FromArray( [GameObject.WorldRotation.Left, GameObject.WorldRotation.Right] ) * 10;
			}
		}

		Holding = item;
	}

	[ConCmd( "debug_drop_item" )]
	static void DebugDropItem( bool byForce = false ) => Instance.ChangeHolding( null, byForce );
}
