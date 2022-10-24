namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Scripted Event Trigger", GroupName = "Cinematic", Description = "Entering this zone will trigger a scripted event, link it to a Scripted Event Camera" )]
public partial class ScriptedEventTrigger : BaseTrigger
{

	[Net, Property, FGDType( "target_destination" ), Description( "End camera position and rotation" )]
	public string EndCameraRef { get; set; }
	[Net, Property, DefaultValue( 2f ), Description( "How fast it reaches the EndCamera (Not seconds)" )]
	public float TransitionSpeed { get; set; }
	[Net, Property, DefaultValue( 4f ), Description( "How long the scripted event lasts before returning camera to the player (Seconds)" )]
	public float TransitionDuration { get; set; }
	[Net, Property, DefaultValue( true ), Description( "Lock the player's inputs")]
	public bool LockInputs { get; set; }

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

			if ( camera.Name == EndCameraRef )
			{

				EndCamera = camera;

			}

		}

	}

	public override void StartTouch( Entity other )
	{

		if ( Host.IsClient ) return;
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

	[ClientRpc]
	public void StartScriptedEvent( string camera, ScriptedEventTrigger trigger )
	{

		Event.Run( "ScriptedEventStart", camera, trigger );

	}

	[ClientRpc]
	public void EndScriptedEvent()
	{

		Event.Run( "ScriptedEventEnd" );

	}

}
