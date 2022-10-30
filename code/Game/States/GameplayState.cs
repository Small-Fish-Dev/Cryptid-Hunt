namespace SpookyJam2022.States;

public class AfterGameStater : BaseState
{
	public override void Init()
	{

		Game.Player.Respawn();

		foreach ( var note in Entity.All.OfType<NotePage>() )
		{

			note.Delete();

		}

	}

	public override void CleanUp()
	{
	}
}
