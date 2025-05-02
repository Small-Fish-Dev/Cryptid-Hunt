namespace CryptidHunt;

public sealed class Key : Item
{
	public override void Attack( Player player )
	{
		if ( player.InteractingWith is LockedChest )
		{
			player.InteractingWith.Interact( player );
			return;
		}
	}
}
