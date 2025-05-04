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

	[Property]
	public GameObject SpitPrefab { get; set; }

	[Property]
	public GameObject BloodParticle { get; set; }

	public bool Alive { get; set; } = true;

	public float JumpscareDistance => 120f;
	public float DetectDistance => 1400f;
	public float StalkingDistance => 700f;
	public float AttackDistance => 500f;
	public float GiveUpDistance => 3600f;
	public float GiveUpAfter => 25f;
	public float AttackAfterStalking => 8f;
	public float AttackAfterStalling => 90f;
	public float WaitUntilNextAttack => 20f;

	[Property]
	public SoundEvent HeartbeatSound { get; set; }
	public SoundHandle Heartbeart { get; set; }

	public Vector3? FirstInterceptPoint()
	{
		var posA = Player.Instance.WorldPosition;
		var velA = Player.Instance.Controller.Velocity.WithZ( 0f );
		var posB = WorldPosition;
		var speedB = CurrentSpeed;

		var toTarget = posA - posB;
		var a = Vector3.Dot( velA, velA ) - speedB * speedB;
		var b = 2f * Vector3.Dot( velA, toTarget );
		var c = Vector3.Dot( toTarget, toTarget );

		// Handle near-zero 'a' (velA magnitude ≈ speedB) as linear:
		if ( MathF.Abs( a ) < 1e-6f )
		{
			// bt + c = 0  →  t = -c/b
			if ( MathF.Abs( b ) < 1e-6f ) return null;
			var tLin = -c / b;
			if ( tLin <= 0f ) return null;
			return posA + velA * tLin;
		}

		var disc = b * b - 4f * a * c;
		if ( disc < 0f ) return null;

		var sqrtDisc = MathF.Sqrt( disc );
		var t1 = (-b + sqrtDisc) / (2f * a);
		var t2 = (-b - sqrtDisc) / (2f * a);

		float t;
		if ( t1 > 0f && t2 > 0f )
			t = MathF.Min( t1, t2 );
		else if ( t1 > 0f )
			t = t1;
		else if ( t2 > 0f )
			t = t2;
		else
			return null;

		return posA + velA * t;
	}

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
				Heartbeart?.Stop();
			}

			if ( value == PolewikState.Pain )
			{
				Heartbeart?.Stop();
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
				Heartbeart?.Stop();
				GameTask.RunInThreadAsync( async () =>
				{
					await Task.DelayRealtimeSeconds( 0.5f );

					Player.Instance.AddCameraShake( 4f, 5f );

					await Task.DelayRealtimeSeconds( 1f );

					ModelRenderer.Set( "howl", true );
					Sound.Play( "howl_far", WorldPosition );
					await Task.DelayRealtimeSeconds( 4.5f );
					CurrentState = PolewikState.AttackPersistent;
				} );
			}

			if ( value == PolewikState.Fleeing )
			{
				Heartbeart?.Stop();
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
			{
				_startedStalking = 0f;
				Heartbeart ??= Sound.Play( HeartbeatSound );
			}

			if ( value == PolewikState.Following || value == PolewikState.AttackPersistent )
			{
				Heartbeart ??= Sound.Play( HeartbeatSound );
				_startedFollowing = 0f;
			}

			if ( value == PolewikState.Attacking )
			{
				_lastAttack = 0f;
				Heartbeart ??= Sound.Play( HeartbeatSound );
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
				Heartbeart?.Stop();
				_lastAttack = 0f;

				Agent.Stop();
				Agent.Velocity = 0f;
				WorldRotation = Rotation.LookAt( Vector3.Direction( WorldPosition, Player.Instance.WorldPosition ) );

				Player.Instance.LockInputs = true;

				ModelRenderer.Set( "attack", true );
				var mouth = ModelRenderer.GetAttachmentObject( "mouth" );
				var spit = SpitPrefab.Clone( mouth.WorldPosition, mouth.WorldRotation );
				spit.SetParent( mouth );

				Sound.Play( "jumpscare", WorldPosition );

				Player.Instance.AddCameraShake( 1.6f, 15f );

				GameTask.RunInThreadAsync( async () =>
				{
					await GameTask.DelaySeconds( 1.2f );

					Player.Instance.HP -= 1;

					await GameTask.DelaySeconds( 1f );

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
		{ PolewikState.Patrolling, 350f },
		{ PolewikState.Stalking, 200f },
		{ PolewikState.Following, 450f },
		{ PolewikState.Attacking, 1800f },
		{ PolewikState.Fleeing, 600f },
		{ PolewikState.Pain, 0f },
		{ PolewikState.Yell, 0f },
		{ PolewikState.AttackPersistent, 450f },
		{ PolewikState.Jumpscare, 0f }

	};
	public float CurrentSpeed => Speeds[CurrentState];

	private float _hp { get; set; } = 100f;

	public float HP
	{
		get => _hp;
		set
		{
			if ( !Alive ) return;

			var damage = _hp - value;
			_hp = value;

			if ( HP <= 0 )
			{
				RagdollModel();
				Sound.Play( "pain", WorldPosition );
				Alive = false;
			}
			else
			{
				if ( damage >= 10f && CurrentState != PolewikState.Pain && CurrentState != PolewikState.Fleeing )
					CurrentState = PolewikState.Pain;

				if ( damage > 0f )
					BloodParticle.Clone( WorldPosition + Vector3.Up * 30f, WorldRotation );
			}
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
		if ( HP <= 0f ) return;

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
			var intercept = FirstInterceptPoint();
			if ( intercept != null )
				NavigateTo( intercept.Value );

			if ( _startedFollowing >= GiveUpAfter )
				CurrentState = PolewikState.Patrolling;

			if ( WithinAttackRange )
				CurrentState = PolewikState.Attacking;

			if ( OutsideDistance )
				CurrentState = PolewikState.Fleeing;
		}

		if ( CurrentState == PolewikState.AttackPersistent )
		{
			var intercept = FirstInterceptPoint();
			if ( intercept != null )
				NavigateTo( intercept.Value );

			if ( _startedFollowing >= GiveUpAfter * 2f )
				CurrentState = PolewikState.Patrolling;

			if ( WithinAttackRange )
				CurrentState = PolewikState.Attacking;

			if ( OutsidePersistentDistance )
				CurrentState = PolewikState.Fleeing;
		}

		if ( CurrentState == PolewikState.Attacking )
		{
			var intercept = FirstInterceptPoint();
			if ( intercept != null )
				NavigateTo( intercept.Value );

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
			Player.Instance.CameraPosition = WorldTransform.PointToWorld( new Vector3( 90f, 0f, 70f ) );
			Player.Instance.CameraRotation = Rotation.LookAt( Vector3.Direction( Player.Instance.Camera.WorldPosition, Camera.WorldPosition ), Vector3.Up );
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

		Agent.Velocity = Vector3.Zero;
		Agent.Enabled = false;
	}
}
