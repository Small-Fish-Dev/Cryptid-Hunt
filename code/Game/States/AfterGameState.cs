namespace SpookyJam2022.States;

public class AfterGameState : BaseState
{
	public override void Init()
	{

		var player = new Player();

		if ( Game.Player != null )
		{

			Game.Player.Delete();
			Game.PlayerClient.Pawn = null;

		}

		Event.Run( "BeginGame" );
		Event.Run( "StartCutscene" );
		Sound.FromScreen( "sounds/music/creepy_radio.sound" ).SetVolume( 3 );

		Game.Player = player;
		Game.PlayerClient.Pawn = player;
		player.Respawn();
		player.Inventory = new( "Backpack", 20, target: Game.PlayerClient );

		foreach ( var note in Entity.All.OfType<NotePage>() )
		{

			note.Delete();

		}

		GameTask.RunInThreadAsync( async () =>
		{

			await GameTask.DelaySeconds( 5f );

			foreach ( var light in Entity.All.OfType<SpotLightEntity>() )
			{

				if ( light.Name.Contains( "brain" ) )
				{

					light.Enabled = true;

				}

			}

		} );

	}

	public override void CleanUp()
	{
	}
}
