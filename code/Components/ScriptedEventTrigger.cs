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

	Vector3 _startPosition;
	Rotation _startRotation;

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Parent.Components.TryGet<Player>( out var player, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		_startPosition = player.Camera.WorldPosition;
		_startRotation = player.Camera.WorldRotation;
		player.LockInputs = LockInputs;

		var polewick = Scene.Components.Get<Polewik>( FindMode.EverythingInSelfAndDescendants );
		if ( polewick.IsValid() )
		{
			polewick.GameObject.Enabled = true;
			polewick.CurrentState = PolewikState.Yell;
		}

		_transitionEnd = Transition.ValueRange.y;

		GameTask.RunInThreadAsync( async () =>
		{
			await Task.DelayRealtimeSeconds( Transition.ValueRange.y + StayDuration );

			player.LockInputs = false;
			Enabled = false;
		} );
	}

	protected override void OnUpdate()
	{
		if ( !Activated ) return;
		if ( !Player.Instance.IsValid() || !Player.Instance.Camera.IsValid() ) return;

		var transition = Transition.Evaluate( (float)_transitionEnd.Fraction ) / Transition.ValueRange.y;
		var endPosition = TargetTransform.WorldPosition;
		var endRotation = TargetTransform.WorldRotation;

		Player.Instance.CameraPosition = Vector3.Lerp( _startPosition, endPosition, transition );
		Player.Instance.CameraRotation = Rotation.Slerp( _startRotation, endRotation, transition );
	}
}
