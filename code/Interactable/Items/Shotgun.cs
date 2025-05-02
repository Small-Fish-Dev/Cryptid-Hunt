namespace CryptidHunt;

public sealed class Shotgun : Item
{
	public override void Attack( Player player )
	{
		if ( player.InteractingWith is LockedDoor )
		{
			player.InteractingWith.Interact( player );
			return;
		}
	}
}
