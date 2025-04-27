namespace CryptidHunt;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }

	protected override void OnStart()
	{
		Instance = this;
	}
}
