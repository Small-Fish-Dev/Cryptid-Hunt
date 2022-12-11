namespace SpookyJam2022.States;

public abstract class BaseState
{
	protected BaseState()
	{
		Game.AssertServer();
		
		Event.Register( this );
	}

	/// <summary>
	/// Called after the previous game state was cleaned up (if we had a state before).
	/// </summary>
	public abstract void Init();

	/// <summary>
	/// Called before the game state is changed. This is used because destructors in Seethe Sharp are unreliable AF.
	/// </summary>
	public abstract void CleanUp();
}
