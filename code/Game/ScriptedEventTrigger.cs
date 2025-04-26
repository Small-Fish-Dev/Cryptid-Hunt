namespace CryptidHunt;

public partial class ScriptedEventTrigger : Component, Component.ITriggerListener
{
	/// <summary>
	/// The position and rotation the camera will move to
	/// </summary>
	[Property]
	public GameObject TargetTransform { get; set; }

	/// <summary>
	/// How fast the camera will move to the target
	/// </summary>
	[Property]
	public float TransitionDuration { get; set; } = 2f;

	/// <summary>
	/// How long the camera stays on target
	/// </summary>
	[Property]
	public float StayDuration { get; set; } = 4f;

	/// <summary>
	/// Lock the player's inputs
	/// </summary>
	[Property]
	public bool LockInputs { get; set; } = true;

	public bool Activated { get; private set; } = false;

	TimeUntil transitionEnd;

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Components.TryGet<Player>( out var player, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		player.LockInputs = LockInputs;
		player.Controller.UseCameraControls = false;

		transitionEnd = TransitionDuration;

		GameTask.RunInThreadAsync( async () =>
		{
			await Task.DelaySeconds( TransitionDuration + StayDuration );
			player.LockInputs = false;
			player.Controller.UseCameraControls = true;
			Enabled = false;
		} );
	}

	protected override void OnUpdate()
	{
		if ( !Activated ) return;
		if ( !Player.Instance.IsValid() || !Player.Instance.Camera.IsValid() ) return;

		var transition = transitionEnd.Fraction;
		var startPosition = Player.Instance.WorldTransform.PointToWorld( Player.Instance.Controller.CameraOffset );
		var endPosition = TargetTransform.WorldPosition;
		var startRotation = Player.Instance.Controller.EyeAngles.ToRotation();
		var endRotation = TargetTransform.WorldRotation;

		Player.Instance.Camera.WorldPosition = Vector3.Lerp( startPosition, endPosition, transition );
		Player.Instance.Camera.WorldRotation = Rotation.Slerp( startRotation, endRotation, transition );
	}
}
