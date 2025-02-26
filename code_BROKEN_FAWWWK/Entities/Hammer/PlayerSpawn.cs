﻿namespace SpookyJam2022;

public enum PlayerSpawnType
{
	Initial,
	Checkpoint
}

[HammerEntity]
[EditorModel( "models/citizen/citizen.vmdl" )]
[Display( Name = "Player Spawn", GroupName = "Spawns", Description = "Initial = When game starts, Checkpoint = When unlocked you'll spawn here upon death" )]
public class PlayerSpawn : Entity
{

	[Property, Description( "Initial = When game starts, Checkpoint = When unlocked you'll spawn here upon death" )]
	public PlayerSpawnType Type { get; set; } = PlayerSpawnType.Initial;
	[Property, Description( "How close the player has to be to unlock this Checkpoint Type" )]
	public float ReachingRange { get; set; } = 100f;
	[Property, Description( "If a player reaches a Checkpoint Type with higher ID than the last it will set it as the current respawn point" )]
	public int CheckpointID { get; set; } = 0;
	public static PlayerSpawn Initial => Entity.All.OfType<PlayerSpawn>().Where( x => x.Type == PlayerSpawnType.Initial ).FirstOrDefault();

	[Event.Tick]
	void findPlayer()
	{

		foreach ( var player in Player.All.OfType<Player>() )
		{

			if ( player.CurrentCheckpoint == null ) continue;
			if ( player.CurrentCheckpoint.CheckpointID > CheckpointID ) continue;

			if ( Vector3.DistanceBetween( player.Position, Position ) <= ReachingRange )
			{

				player.CurrentCheckpoint = this;

			}

		}

	}

	public static PlayerSpawn WithID( int id )
	{

		return Entity.All
			.OfType<PlayerSpawn>()
			.Where( x => x.CheckpointID == id )
			.FirstOrDefault();

	}

}
