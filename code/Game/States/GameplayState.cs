﻿namespace SpookyJam2022.States;

public class GameplayState : BaseState
{
	public override void Init()
	{
		var player = new Player();
		Game.Player = player;
		Game.PlayerClient.Pawn = player;
		player.Respawn();
		player.Inventory = new("Backpack", 30, target: Game.PlayerClient);

		foreach ( var ent in Entity.All ) // Find camera (shitt)
		{

			if ( ent is not ScriptedEventCamera camera ) continue;

			if ( camera.Name.Contains( "MainMenu" ) )
			{

				player.OverrideCamera = camera;
				player.ScriptedEvent = true;
				player.LockInputs = true;

			}

		}
		
		Event.Run( "BeginGame" ); // glass break
	}

	public override void CleanUp()
	{
	}
}