namespace CryptidHunt;

public sealed class Medkit : Item
{
	public override void Use( Player player )
	{
		player.HP = 3;
		GameObject.Destroy();
	}
}
