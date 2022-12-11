using System.Diagnostics.CodeAnalysis;

namespace SpookyJam2022.States;

public class AfterGameState : BaseState
{
	public override void Init()
	{
		var count = 0;

		foreach( var ply in Entity.All.OfType<Player>() )
		{

			ply.Initial();
			ply.FlashLightOn = false;
			count++;

		}

		if ( count == 0 )
		{

			var player = new Player();
			CryptidHunt.Player = player;
			CryptidHunt.PlayerClient.Pawn = player;
			player.Initial();
			player.Inventory = new( "Backpack", 20, target: CryptidHunt.PlayerClient );

		}

		Event.Run( "BeginGame" );
		Event.Run( "StartCutscene" );

		foreach ( var note in Entity.All.OfType<NotePage>() )
		{

			if ( note.Name.Contains( "Brain" ) )
			{

				note.EnableDrawing = true;
				note.EnableAllCollisions = true;

			}
			else
			{

				note.Delete();


			}

		}

	}
	[Event( "BrainReveal" )]
	public void OnReveal()
	{

		foreach ( var light in Entity.All.OfType<SpotLightEntity>() )
		{

			if ( light.Name.Contains( "brain" ) )
			{

				light.Enabled = true;

			}

		}

		Sound.FromScreen( "sounds/music/creepy_radio.sound" ).SetVolume( 3 );

	}

	public override void CleanUp()
	{
	}
}
