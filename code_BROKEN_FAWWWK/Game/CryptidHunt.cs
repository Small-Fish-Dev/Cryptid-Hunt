global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text.Json.Serialization;
global using System.ComponentModel.DataAnnotations;
global using Editor;
global using System.IO;
global using System.Threading.Tasks;
global using System.Text.Json;
global using Sandbox.Effects;
global using Sandbox.Component;
using SpookyJam2022.States;

namespace SpookyJam2022;

public partial class CryptidHunt : GameManager
{
	public enum GameState
	{
		MainMenu,
		NextBot,
		Gameplay
	}

	public static CryptidHunt Instance { get; private set; }
	public static IClient PlayerClient { get; private set; }
	public static BasePlayer Player { get; set; }

	public static BaseState State
	{
		get => _state;
		set
		{
			Game.AssertServer();

			if ( _state is not null )
				_state.CleanUp();

			_state = value;
			_state.Init();
		}
	}

	private static BaseState _state = null;

	public CryptidHunt()
	{
		Instance = this;
		InitializeScreenEffects();

		if ( Game.IsClient )
		{
			_ = new HUD();
		}
	}

	[Event.Entity.PostSpawn]
	public void LevelLoaded()
	{
	
		Precache.Add( "particles/bigblood.vpcf" );
		Precache.Add( "particles/spit01.vpcf" );
		Precache.Add( "particles/spitdrops.vpcf" );
		Precache.Add( "particles/spitpuff.vpcf" );

	}

	public override void ClientJoined( IClient client )
	{
		// Limit to one player only.
		if ( PlayerClient != null )
		{
			client.Kick();
			return;
		}

		PlayerClient = client;
		State = new MainMenuState();

	}

	public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
	{
		cl.Pawn?.Delete();
		cl.Pawn = null;
	}

	ScriptedEventCamera cam;
	/*
	public override void BuildCamera( CameraSetup camSetup )
	{
		if ( Game.LocalPawn == null )
		{
			if ( cam == null )
			{
				var cameras = Entity.All.OfType<ScriptedEventCamera>().ToList();
				for ( int i = 0; i < cameras.Count; i++ )
				{
					var camera = cameras[i];
					if ( camera == null ) continue;

					if ( camera.Name.Contains( "MainMenu" ) )
					{
						cam = camera;
						break;
					}
				}
			} 
			else
			{
				camSetup.Position = cam.Position;
				camSetup.Rotation = cam.Rotation;
				camSetup.FieldOfView = 70f;
			}
		}

		Game.LocalPawn?.PostCameraSetup( ref camSetup );
		return camSetup;
	}
	*/
	public override void Simulate( IClient cl )
	{
		if ( cl.Pawn is not BasePlayer pawn ) return;

		pawn.Simulate( cl );
	}

	public override void FrameSimulate( IClient cl )
	{
		if ( cl.Pawn is not BasePlayer pawn ) return;

		pawn.FrameSimulate( cl );
	}

	public override void Shutdown()
	{
		if ( Instance == this )
			Instance = null;

		Instance?.Delete();
	}

	public override bool CanHearPlayerVoice( IClient source, IClient receiver )
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

	[ClientRpc]
	public void StartZoneHint( string Name )
	{
		Event.Run( "ShowArea", Name );
	}

	[ClientRpc]
	public void StartInputHint( string buttonHint, string hint )
	{
		Event.Run( "InputHint", buttonHint, hint );
	}


	public override void RenderHud()
	{
		base.RenderHud();
		Event.Run( "Render" );
	}
}
