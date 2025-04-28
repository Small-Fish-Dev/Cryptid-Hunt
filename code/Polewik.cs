using Sandbox;
using Sandbox.UI;
using Sandbox.VR;
using System.IO;
using static Sandbox.SceneModel;

namespace CryptidHunt;

public enum PolewikState
{
	Idle,
	Patrolling,
	Stalking,
	Following,
	Attacking,
	Fleeing,
	Pain,
	Yell,
	AttackPersistent,
	Jumpscare
}

public partial class Polewik : Component
{
	[Property]
	public SkinnedModelRenderer ModelRenderer { get; set; }

	[Property]
	public GameObject Camera { get; set; }

	public float JumpscareDistance => 120f;
	public float DetectDistance => 1400f;
	public float StalkingDistance => 700f;
	public float AttackDistance => 500f;
	public float GiveUpDistance => 3600f;
	public float GiveUpAfter => 25f;
	public float AttackAfterStalking => 8f;
	public float AttackAfterStalling => 90f;
	public float WaitUntilNextAttack => 20f;

	PolewikState currentState { get; set; } = PolewikState.Patrolling;

	public PolewikState CurrentState
	{
		get => currentState;
		set
		{
			currentState = value;

			if ( value == PolewikState.Patrolling )
			{
				NavigateTo( NearestNode.WorldPosition );
				CurrentPathId = PatrolPath.IndexOf( NearestNode );
			}

			if ( value == PolewikState.Pain )
			{
				_lastAttack = 0f;

				Sound.Play( "pain", WorldPosition );

				Task.RunInThreadAsync( async () =>
				{
					ModelRenderer.Set( "growl", true );
					await Task.DelaySeconds( 0.1f );
					ModelRenderer.Set( "growl", false );
					await Task.DelaySeconds( 0.9f );
					CurrentState = PolewikState.Fleeing;
				} );

			}

			if ( value == PolewikState.Yell )
			{
				GameTask.RunInThreadAsync( async () =>
				{
					await GameTask.DelaySeconds( 0.5f );

					Player.Instance.CameraShake = 2f;
					Player.Instance.ShakeIntensity = 30f;

					ModelRenderer.Set( "howl", true );
					await GameTask.DelaySeconds( 3.5f );
					CurrentState = PolewikState.AttackPersistent;
				} );
			}

			if ( value == PolewikState.Fleeing )
			{
				_lastAttack = 0f;

				TargetPosition = NearestNode.WorldPosition;
				NavigateTo( NearestNode.WorldPosition );
				CurrentPathId = PatrolPath.IndexOf( NearestNode );

				GameTask.RunInThreadAsync( async () =>
				{
					await GameTask.DelaySeconds( Game.Random.Float( 3f, 6f ) );
					CurrentState = PolewikState.Patrolling;
				} );
			}

			if ( value == PolewikState.Stalking )
				_startedStalking = 0f;

			if ( value == PolewikState.Following )
				_startedFollowing = 0f;

			if ( value == PolewikState.AttackPersistent )
				_startedFollowing = 0f;

			if ( value == PolewikState.Attacking )
			{
				_lastAttack = 0f;
				ModelRenderer.Set( "leap", true );

				Sound.Play( "jump", WorldPosition );

				GameTask.RunInThreadAsync( async () =>
				{
					await GameTask.DelaySeconds( 0.05f );
					ModelRenderer.Set( "leap", false );
					await GameTask.DelaySeconds( 0.9f );

					if ( CurrentState == PolewikState.Attacking )
						CurrentState = PolewikState.Following;
				} );
			}

			if ( value == PolewikState.Jumpscare )
			{
				_lastAttack = 0f;

				Agent.Stop();
				Agent.Velocity = 0f;
				WorldRotation = Rotation.LookAt( Vector3.Direction( WorldPosition, Player.Instance.WorldPosition ) );

				Player.Instance.LockInputs = true;

				ModelRenderer.Set( "attack", true );

				Sound.Play( "jumpscare", WorldPosition );

				Player.Instance.CameraShake = 1.6f;
				Player.Instance.ShakeIntensity = 20f;

				GameTask.RunInThreadAsync( async () =>
				{
					await GameTask.DelaySeconds( 1.2f );

					Player.Instance.HP -= 1;

					await GameTask.DelaySeconds( 0.45f );

					if ( CurrentState == PolewikState.Jumpscare )
						CurrentState = PolewikState.Fleeing;

					Player.Instance.LockInputs = false;
					// TODO: Make player camera follow the camera attachment
				} );
			}
		}
	}

	[Property]
	public List<GameObject> PatrolPath { get; set; } = new List<GameObject>();
	public int CurrentPathId { get; set; } = 0;
	public Vector3 TargetPosition { get; set; }

	[Property]
	public NavMeshAgent Agent { get; set; }

	public Dictionary<PolewikState, float> Speeds = new()
	{
		{ PolewikState.Idle, 0f },
		{ PolewikState.Patrolling, 450f },
		{ PolewikState.Stalking, 300f },
		{ PolewikState.Following, 550f },
		{ PolewikState.Attacking, 1800f },
		{ PolewikState.Fleeing, 750f },
		{ PolewikState.Pain, 0f },
		{ PolewikState.Yell, 0f },
		{ PolewikState.AttackPersistent, 550f },
		{ PolewikState.Jumpscare, 0f }

	};
	public float CurrentSpeed => Speeds[CurrentState];

	private float _hp { get; set; } = 100f;

	public float HP
	{
		get => _hp;
		set
		{
			var damage = _hp - value;
			_hp = value;

			if ( HP <= 0 )
			{
				RagdollModel();
				Sound.Play( "pain", WorldPosition );
			}

			if ( damage >= 10f && CurrentState != PolewikState.Pain && CurrentState != PolewikState.Fleeing )
				CurrentState = PolewikState.Pain;
		}
	}

	TimeSince _lastCalculatedPath;
	TimeSince _startedStalking;
	TimeSince _startedFollowing;
	TimeSince _lastAttack;
	public GameObject ClosestNodeTo( Vector3 pos ) => PatrolPath.OrderBy( x => x.WorldPosition.Distance( pos ) ).FirstOrDefault();
	public GameObject NearestNode => ClosestNodeTo( WorldPosition );
	public GameObject FurthestNode => PatrolPath.OrderBy( x => x.WorldPosition.Distance( WorldPosition ) ).LastOrDefault();
	public bool WithinAttackRange => Player.Instance.WorldPosition.Distance( WorldPosition ) <= AttackDistance && Math.Abs( Player.Instance.WorldPosition.z - WorldPosition.z ) <= 200f;
	public bool OutsideDistance => Player.Instance.WorldPosition.Distance( WorldPosition ) >= GiveUpDistance || Math.Abs( Player.Instance.WorldPosition.z - WorldPosition.z ) > 200f;
	public bool OutsidePersistentDistance => Player.Instance.WorldPosition.Distance( WorldPosition ) >= GiveUpDistance * 4f || Math.Abs( Player.Instance.WorldPosition.z - WorldPosition.z ) > 200f;

	protected override void OnStart()
	{
		if ( ModelRenderer.IsValid() )
			ModelRenderer.OnFootstepEvent += OnFootstepEvent;
	}

	protected override void OnFixedUpdate()
	{
		ComputeAnimation();
		Agent.MaxSpeed = CurrentSpeed;

		/*
		DebugOverlay.Sphere( Position, JumpscareDistance, Color.Red, 0f, false );
		DebugOverlay.Sphere( Position, DetectDistance, Color.Green );
		DebugOverlay.Sphere( Position, StalkingDistance, Color.Yellow );
		DebugOverlay.Sphere( Position, AttackDistance, Color.Orange );
		DebugOverlay.Sphere( Position, GiveUpDistance, Color.Blue );*/

		if ( CurrentState != PolewikState.Idle && CurrentState != PolewikState.Pain )
			ComputePath();

		if ( CurrentState == PolewikState.Patrolling )
		{
			NavigateTo( TargetPosition );

			if ( _lastAttack >= WaitUntilNextAttack && Player.Instance.WorldPosition.Distance( WorldPosition ) <= DetectDistance )
				CurrentState = PolewikState.Stalking;

			if ( _lastAttack >= AttackAfterStalling )
				CurrentState = PolewikState.Yell;

			Agent.UpdateRotation = true;
		}

		if ( CurrentState == PolewikState.Stalking )
		{
			NavigateTo( ClosestNodeTo( Player.Instance.WorldPosition ).WorldPosition );
			Agent.UpdateRotation = false;
			WorldRotation = Rotation.LookAt( Vector3.Direction( WorldPosition, Player.Instance.WorldPosition ) );

			if ( Player.Instance.WorldPosition.Distance( WorldPosition ) <= StalkingDistance || _startedStalking >= AttackAfterStalking )
			{
				Sound.Play( "scream_scare", WorldPosition );
				CurrentState = PolewikState.Following;
			}

			if ( OutsideDistance )
				CurrentState = PolewikState.Fleeing;

			var lookingTrace = Scene.Trace.Sphere( 200f, Player.Instance.Camera.WorldPosition, Player.Instance.Camera.WorldPosition + Player.Instance.Camera.WorldRotation.Forward * 2000f )
				.WithTag( "Polewik" )
				.IgnoreStatic()
				.IgnoreGameObjectHierarchy( Player.Instance.GameObject )
				.Run();

			if ( lookingTrace.Hit && lookingTrace.GameObject == GameObject )
				CurrentState = PolewikState.Following;
		}

		if ( CurrentState == PolewikState.Following )
		{
			NavigateTo( Player.Instance.WorldPosition );

			if ( _startedFollowing >= GiveUpAfter )
				CurrentState = PolewikState.Patrolling;

			if ( WithinAttackRange )
				CurrentState = PolewikState.Attacking;

			if ( OutsideDistance )
				CurrentState = PolewikState.Fleeing;
		}

		if ( CurrentState == PolewikState.AttackPersistent )
		{
			NavigateTo( Player.Instance.WorldPosition );

			if ( _startedFollowing >= GiveUpAfter * 2f )
				CurrentState = PolewikState.Patrolling;

			if ( WithinAttackRange )
				CurrentState = PolewikState.Attacking;

			if ( OutsidePersistentDistance )
				CurrentState = PolewikState.Fleeing;
		}

		if ( CurrentState == PolewikState.Attacking )
		{
			if ( Player.Instance.WorldPosition.Distance( WorldPosition ) <= 100f )
				CurrentState = PolewikState.Jumpscare;

			if ( OutsideDistance )
				CurrentState = PolewikState.Fleeing;
		}

		if ( CurrentState == PolewikState.Fleeing && PatrolPath != null )
		{
			NavigateTo( TargetPosition );
		}
	}

	protected override void OnUpdate()
	{
		if ( CurrentState == PolewikState.Jumpscare )
		{
			Player.Instance.CameraPosition = Camera.WorldPosition;
			Player.Instance.CameraRotation = Camera.WorldRotation;
		}
	}

	public void OnFootstepEvent( SceneModel.FootstepEvent footstepEvent )
	{
		var footTrace = Scene.Trace.Ray( WorldPosition, WorldPosition + Vector3.Down * 10f )
			.Radius( 2f )
			.IgnoreDynamic()
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( !footTrace.Hit ) return;

		var tag = footTrace.Tags
			.Where( x => x != "solid" && x != "world" )
			.FirstOrDefault();

		var sound = tag switch
		{
			"metal" => "footstep-metal",
			"grass" => "footstep-grass",
			"dirt" => "footstep-dirt",
			_ => "footstep-concrete"
		};

		Sound.Play( sound, footTrace.EndPosition ).Volume *= Agent.Velocity.WithZ( 0f ).Length / 7f;
	}

	public virtual void ComputeAnimation()
	{
		ModelRenderer.Set( "speed", Agent.Velocity.Length / 3 );

		if ( CurrentState == PolewikState.Following ||
			CurrentState == PolewikState.Stalking ||
			CurrentState == PolewikState.Attacking ||
			CurrentState == PolewikState.AttackPersistent ||
			CurrentState == PolewikState.Jumpscare )
		{

			var local = WorldTransform.PointToLocal( Player.Instance.WorldPosition );
			ModelRenderer.Set( "lookat", local.WithX( Math.Max( local.x, 0 ) ) + Vector3.Forward * 300f );
		}
	}

	public virtual bool NavigateTo( Vector3 pos )
	{
		Agent.MoveTo( pos );
		return true;
	}

	public virtual void ComputePath()
	{
		if ( PatrolPath == null ) return;

		TargetPosition = PatrolPath[CurrentPathId].WorldPosition;

		if ( WorldPosition.Distance( TargetPosition ) < MathF.Max( Agent.Velocity.Length, 100f ) * 20f * Time.Delta )
			CurrentPathId = (CurrentPathId + 1) % PatrolPath.Count;
	}

	public void RagdollModel()
	{
		if ( !ModelRenderer.IsValid() ) return;

		var ragdoll = ModelRenderer.GameObject.AddComponent<ModelPhysics>();
		ragdoll.Renderer = ModelRenderer;
		ragdoll.Model = ModelRenderer.Model;

		Agent.Enabled = false;
	}


}
