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
			Game.Player = player;
			Game.PlayerClient.Pawn = player;
			player.Initial();
			player.Inventory = new( "Backpack", 20, target: Game.PlayerClient );

		}

		Event.Run( "BeginGame" );
		Event.Run( "StartCutscene" );

		foreach ( var note in Entity.All.OfType<NotePage>() )
		{

			note.Delete();

		}

		foreach ( var ent in Entity.All.OfType<ModelEntity>() )
		{ 

			if ( !ent.Name.Contains( "computer" ) ) continue;

			var attachmentPos = ent.GetAttachment( "screen" )?.Position ?? Vector3.Zero;

			var page = new NotePage()
			{
				Position = attachmentPos + Vector3.Right * 3f,
				Text = "Great job on your first assignment,\nwe have sent you the payment, that should cover the window;\nSpeaking of, your next assignment starts now.\n\nLook outside your window",


			};

			page.Rotation = Rotation.From( new Angles( -60f, 90f, 0f ) );

			break;
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
