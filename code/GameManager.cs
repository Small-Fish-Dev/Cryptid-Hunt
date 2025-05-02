namespace CryptidHunt;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }

	public bool ReadInitialNote { get; set; } = false;

	protected override void OnStart()
	{
		Instance = this;
	}
}
