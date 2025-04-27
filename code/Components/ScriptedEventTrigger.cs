namespace CryptidHunt;

public partial class ScriptedEventTrigger : Component, Component.ITriggerListener
{
	/// <summary>
	/// The position and rotation the camera will move to
	/// </summary>
	[Property]
	public GameObject TargetTransform { get; set; }

	[Property]
	public Curve Transition { get; set; }

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

	RealTimeUntil _transitionEnd;

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Parent.Components.TryGet<Player>( out var player, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		player.LockInputs = LockInputs;
		player.Controller.UseCameraControls = false;

		_transitionEnd = Transition.ValueRange.y;

		GameTask.RunInThreadAsync( async () =>
		{
			await Task.DelayRealtimeSeconds( Transition.ValueRange.y + StayDuration );

			player.LockInputs = false;
			player.Controller.UseCameraControls = true;
			Enabled = false;
		} );
	}

	protected override void OnUpdate()
	{
		if ( !Activated ) return;
		if ( !Player.Instance.IsValid() || !Player.Instance.Camera.IsValid() ) return;

		var transition = Transition.Evaluate( (float)_transitionEnd.Fraction ) / Transition.ValueRange.y;
		var startPosition = Player.Instance.WorldTransform.PointToWorld( Player.Instance.CameraOffset );
		var endPosition = TargetTransform.WorldPosition;
		var startRotation = Player.Instance.Controller.EyeAngles.ToRotation();
		var endRotation = TargetTransform.WorldRotation;

		Player.Instance.Camera.WorldPosition = Vector3.Lerp( startPosition, endPosition, transition );
		Player.Instance.Camera.WorldRotation = Rotation.Slerp( startRotation, endRotation, transition );
	}
}
