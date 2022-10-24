global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text.Json.Serialization;
global using System.ComponentModel.DataAnnotations;
global using SandboxEditor;
global using System.IO;
global using System.Threading.Tasks;
global using System.Text.Json;
global using Sandbox.Effects;
global using Sandbox.Component;

namespace SpookyJam2022;

public partial class Game : GameBase
{
	public static Game Instance { get; private set; }
	public static Player Player { get; private set; }
	
	public Game()
	{
		Instance = this;
		Event.Run( "GameStart" ); //TODO: Set this on the menu
	}

	public override void ClientJoined( Client client )
	{
		// Limit to one player only.
		if ( Player != null )
		{
			client.Kick();
			return;
		}

		Player = new Player();
		client.Pawn = Player;
		Player.Respawn();
		Player.Inventory = new( "Backpack", 6, 8, target: client );
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		cl.Pawn?.Delete();
		cl.Pawn = null;
	}

	public override CameraSetup BuildCamera( CameraSetup camSetup )
	{
		Local.Pawn?.PostCameraSetup( ref camSetup );
		return camSetup;
	}

	public override void Simulate( Client cl )
	{
		if ( !cl.Pawn.IsValid() ) return;
		if ( !cl.Pawn.IsAuthority ) return;

		cl.Pawn?.Simulate( cl );
	}

	public override void FrameSimulate( Client cl )
	{
		if ( !cl.Pawn.IsValid() ) return;
		if ( !cl.Pawn.IsAuthority ) return;

		cl.Pawn?.FrameSimulate( cl );
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );
		Local.Pawn?.BuildInput( input );
	}

	public override void Shutdown()
	{
		
	}

	public override bool CanHearPlayerVoice( Client source, Client receiver )
	{
		return false;
	}

	public override void PostLevelLoaded()
	{
		
	}

	[ClientRpc]
	public void StartBlackScreen()
	{

		Event.Run( "BlackScreen" );

	}


}
