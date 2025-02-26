﻿using Sandbox.Internal;

namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Scripted Event Trigger", GroupName = "Cinematic", Description = "Entering this zone will trigger a scripted event, link it to a Scripted Event Camera" )]
public partial class ScriptedEventTrigger : BaseTrigger
{

	[Net, Property, FGDType( "target_destination" ), Description( "End camera position and rotation" )]
	public string EndCameraRef { get; set; }
	[Net, Property, Description( "How fast it reaches the EndCamera (Not seconds)" )]
	public float TransitionSpeed { get; set; } = 2f;
	[Net, Property, Description( "How long the scripted event lasts before returning camera to the player (Seconds)" )]
	public float TransitionDuration { get; set; } = 4f;
	[Net, Property, Description( "Lock the player's inputs" )]
	public bool LockInputs { get; set; } = true;

	[Net] public ScriptedEventCamera EndCamera { get; set; }
	[Net] public bool Active { get; set; } = false;
	[Net] public bool Activateable { get; set; } = true;

	public ScriptedEventTrigger() { }
	public override void Spawn()
	{

		base.Spawn();

		Transmit = TransmitType.Always; // We need this for ClientRPC

		foreach ( var ent in Entity.All )
		{

			if ( ent is not ScriptedEventCamera camera ) continue;

			if ( camera.Name == EndCameraRef || camera.Name.Contains( EndCameraRef ) )
			{

				EndCamera = camera;

			}

		}

	}

	public override void StartTouch( Entity other )
	{

		if ( Game.IsClient ) return;
		if ( !Activateable ) return;
		if ( other is not Player player ) return;

		base.Touch( other );

		Activateable = false;
		Active = true;

		player.LockInputs = LockInputs;

		StartScriptedEvent( EndCameraRef, this );

		GameTask.RunInThreadAsync( async () =>
		{
			await Task.DelaySeconds( TransitionDuration );

			Active = false;
			player.LockInputs = false;

			EndScriptedEvent();

		} );

	}

	public void StartScriptedEvent( string camera, ScriptedEventTrigger trigger )
	{

		Event.Run( "ScriptedEventStart", camera, trigger );

	}

	public void EndScriptedEvent()
	{

		Event.Run( "ScriptedEventEnd" );

	}

}
