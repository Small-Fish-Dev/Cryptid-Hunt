namespace CryptidHunt;

public partial class ScriptedEventTrigger : Component, Component.ICollisionListener
{
	[Property, Description( "End camera position and rotation" )]
	public GameObject CameraTargetTransform { get; set; }

	[Property, Description( "How fast it reaches the EndCamera (Not seconds)" )]
	public float TransitionSpeed { get; set; } = 2f;

	[Property, Description( "How long the scripted event lasts before returning camera to the player (Seconds)" )]
	public float TransitionDuration { get; set; } = 4f;

	[Property, Description( "Lock the player's inputs" )]
	public bool LockInputs { get; set; } = true;

	public ScriptedEventCamera EndCamera { get; set; }
	public bool Activated { get; set; } = false;
	public bool Activateable { get; set; } = true;

	public override void

	public override void StartTouch( Entity other )
	{

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
