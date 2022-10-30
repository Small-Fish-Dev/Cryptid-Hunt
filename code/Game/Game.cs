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
using SpookyJam2022.States;

namespace SpookyJam2022;

public partial class Game : GameBase
{
	public enum GameState
	{
		MainMenu,
		NextBot,
		Gameplay
	}

	public static Game Instance { get; private set; }
	public static Client PlayerClient { get; private set; }
	public static BasePlayer Player { get; set; }

	public static BaseState State
	{
		get => _state;
		set
		{
			Host.AssertServer();

			if ( _state is not null )
				_state.CleanUp();

			_state = value;
			_state.Init();
		}
	}

	private static BaseState _state = null;

	public Game()
	{
		Instance = this;
		Event.Run( "GameStart" );
	}

	[Event.Entity.PostSpawn]
	public void LevelLoaded()
	{
	
		Precache.Add( "sounds/ambient/ambient_wind_loop.sound" );
		Precache.Add( "sounds/ambient/crows.sound" );
		Precache.Add( "sounds/ambient/leaves_rustle.sound" );
		Precache.Add( "sounds/items/beartrap_set.sound" );
		Precache.Add( "sounds/items/beartrap_trigger.sound" );
		Precache.Add( "sounds/items/chest_locked.sound" );
		Precache.Add( "sounds/items/chest_open.sound" );
		Precache.Add( "sounds/items/crowbar.sound" );
		Precache.Add( "sounds/items/door_open.sound" );
		Precache.Add( "sounds/items/medkit.sound" );
		Precache.Add( "sounds/items/metal_bar.sound" );
		Precache.Add( "sounds/items/metal_door_creak.sound" );
		Precache.Add( "sounds/items/page_close.sound" );
		Precache.Add( "sounds/items/page_open.sound" );
		Precache.Add( "sounds/items/pickup.sound" );
		Precache.Add( "sounds/misc/breathe_in.sound" );
		Precache.Add( "sounds/misc/breathe_out.sound" );
		Precache.Add( "sounds/misc/car_intro.sound" );
		Precache.Add( "sounds/misc/glass_break_intro.sound" );
		Precache.Add( "sounds/misc/heart_beat.sound" );
		Precache.Add( "sounds/music/creepy_radio.sound" );
		Precache.Add( "sounds/music/dayofchaos.sound" );
		Precache.Add( "sounds/music/piano_intro.sound" );
		Precache.Add( "sounds/music/weird_flutes.sound" );
		Precache.Add( "sounds/polewik/breathe.sound" );
		Precache.Add( "sounds/polewik/hit.sound" );
		Precache.Add( "sounds/polewik/howl_far.sound" );
		Precache.Add( "sounds/polewik/jump.sound" );
		Precache.Add( "sounds/polewik/jumpscare.sound" );
		Precache.Add( "sounds/polewik/pain.sound" );
		Precache.Add( "sounds/polewik/scream_scare.sound" );
		Precache.Add( "sounds/scary/creepy_sound.sound" );
		Precache.Add( "sounds/scary/door_slam.sound" );
		Precache.Add( "sounds/scary/slam_flute.sound" );
		Precache.Add( "sounds/scary/static_loop.sound" );
		Precache.Add( "sounds/scary/string_high.sound" );
		Precache.Add( "sounds/ui/button_click.sound" );
		Precache.Add( "sounds/footsteps/footstep-dirt.sound" );
		Precache.Add( "sounds/footsteps/footstep-grass.sound" );
		Precache.Add( "sounds/footsteps/footstep-wood.sound" );
		Precache.Add( "particles/bigblood.vpcf" );
		Precache.Add( "particles/spit01.vpcf" );
		Precache.Add( "particles/spitdrops.vpcf" );
		Precache.Add( "particles/spitpuff.vpcf" );

	}

	public override void ClientJoined( Client client )
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

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		cl.Pawn?.Delete();
		cl.Pawn = null;
	}

	ScriptedEventCamera cam;

	public override CameraSetup BuildCamera( CameraSetup camSetup )
	{
		if ( Local.Pawn == null )
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
		Event.Run( "BuildInput", input );
		Local.Pawn?.BuildInput( input );
	}

	public override void Shutdown()
	{
		if ( Instance == this )
			Instance = null;

		Instance?.Delete();
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
