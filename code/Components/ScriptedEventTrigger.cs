namespace CryptidHunt;

public partial class ScriptedEventTrigger : Component, Component.ITriggerListener
{
	public class PolewikTarget
	{
		public GameObject SpawnPoint { get; set; }
		public GameObject CameraTarget { get; set; }
	}

	/// <summary>
	/// The position and rotation the camera will move to
	/// </summary>
	[Property]
	public GameObject TargetTransform { get; set; }

	[Property]
	public PolewikTarget[] PolewikSpawnPoints { get; set; } = [];

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

		var farthestSpawnPoint = PolewikSpawnPoints.Aggregate( ( i1, i2 ) =>
			i1.SpawnPoint.WorldPosition.DistanceSquared( player.WorldPosition ) > i2.SpawnPoint.WorldPosition.DistanceSquared( player.WorldPosition ) ? i1 : i2 );
		if ( farthestSpawnPoint.CameraTarget.IsValid() )
		{
			TargetTransform = farthestSpawnPoint.CameraTarget;
		}

		var polewick = Scene.Components.Get<Polewik>( FindMode.EverythingInSelfAndDescendants );
		if ( polewick.IsValid() )
		{
			if ( farthestSpawnPoint.SpawnPoint.IsValid() )
			{
				polewick.WorldPosition = farthestSpawnPoint.SpawnPoint.WorldPosition;
				polewick.WorldRotation = farthestSpawnPoint.SpawnPoint.WorldRotation;
			}

			polewick.GameObject.Enabled = true;
			polewick.CurrentState = PolewikState.Yell;
		}

		_transitionEnd = Transition.ValueRange.y;

		GameTask.RunInThreadAsync( async () =>
		{
			await Task.MainThread();
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
