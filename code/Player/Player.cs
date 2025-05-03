namespace CryptidHunt;

public partial class Player : Component
{
	public static Player Instance { get; private set; }

	[Property]
	public PlayerController Controller { get; set; }

	[Property]
	public CameraComponent Camera { get; set; }

	public Interactable InteractingWith { get; set; }

	public float RunSpeed { get; set; }
	public float Stamina { get; set; } = 100f;
	public bool Running { get; set; } = false;
	public TimeSince LastRan { get; set; } = 0f;
	public TimeSince LastInteraction { get; set; } = 0f;
	public bool LockInputs { get; set; } = false;
	public int HP { get; set; } = 3;

	protected override void OnStart()
	{
		Instance = this;

		if ( Controller.IsValid() )
		{
			RunSpeed = Controller.RunSpeed;
			Controller.RunSpeed = Controller.WalkSpeed;
			_cameraOffset = CameraOffset;
		}
	}

	public TimeUntil NextInteraction { get; set; }
	public SceneTraceResult InteractTrace { get; set; }

	protected override void OnFixedUpdate()
	{
		if ( !Controller.IsValid() ) return;

		if ( !LockInputs && Input.Down( "Run" ) && Stamina > 0 && (Running || LastRan >= 1f) )
			Running = true;
		else
			Running = false;

		if ( Running )
		{
			Stamina = Math.Clamp( Stamina - Time.Delta * 10f, 0f, 100f );
			Controller.RunSpeed = RunSpeed;
			LastRan = 0f;
		}
		else
		{
			Controller.RunSpeed = Controller.WalkSpeed;
			Stamina = Math.Clamp( Stamina + Time.Delta * 10f, 0f, 100f );
		}

		if ( !LockInputs )
		{
			Controller.UseInputControls = true;

			if ( Input.Pressed( "Flashlight" ) )
				SetFlashLight( !FlashLightOn, true );

			if ( Input.Pressed( "attack1" ) )
			{
				if ( NextInteraction )
				{
					if ( Holding.IsValid() )
					{
						NextInteraction = 0.5f;
						LastInteraction = 0;
						Holding.Attack( this );
					}
				}

			}

		}
		else
		{
			Controller.UseInputControls = false;
			Controller.WishVelocity = 0f;
		}

		InteractTrace = Scene.Trace.Ray( Camera.WorldPosition, Camera.WorldPosition + Camera.WorldRotation.Forward * 150f )
			.Radius( 5f )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( InteractTrace.Hit && InteractTrace.GameObject.Components.TryGet<Interactable>( out var interactable ) )
			InteractingWith = interactable;
		else
			InteractingWith = null;

		if ( Input.Pressed( "use" ) && NextInteraction )
		{
			NextInteraction = 0.2f;

			if ( InteractingWith.IsValid() )
			{
				InteractingWith.Interact( this );
				LastInteraction = 0;
				AddCameraShake( 0.2f, 3f );
			}
		}

		HandleBreathing();
		HandleFootsteps();
	}

	protected override void OnUpdate()
	{
		SetupCamera();
	}

	public void Respawn()
	{

		//CurrentCheckpoint ??= PlayerSpawn.Initial;

		//EnableAllCollisions = true;
		//EnableDrawing = true;

		//WorldPosition = CurrentCheckpoint.Position;
		//Rotation = CurrentCheckpoint.Rotation;

	}

	TimeSince _lastBreath;
	bool _breatheIn;

	public void HandleBreathing()
	{
		if ( HP <= 0f || LockInputs ) return;

		var breathTime = Running ? 1f : 1.5f;

		if ( _lastBreath >= breathTime )
		{
			_breatheIn = !_breatheIn;
			_lastBreath = 0f;

			var sound = Sound.Play( _breatheIn ? "breathe_in" : "breathe_out", WorldPosition );
			sound.Volume *= Running ? 10f : 5f;
			sound.Pitch *= Running ? 1.1f : 1f;

		}
	}

	TimeUntil _nextFootstep;
	TimeSince _lastStep = 0f;

	public void HandleFootsteps()
	{
		if ( !Controller.IsValid() ) return;
		if ( LockInputs ) return;

		if ( Controller.IsClimbing && _nextFootstep )
		{
			_nextFootstep = 0.3f;
			Sound.Play( "footstep-metal", WorldPosition ).Volume *= 7;
		}

		if ( _lastStep <= (Running ? 0.3f : 0.6f) ) return;
		_lastStep = 0f;

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

		Sound.Play( sound, footTrace.EndPosition ).Volume *= Controller.Velocity.WithZ( 0f ).Length / 15f;
	}
}
