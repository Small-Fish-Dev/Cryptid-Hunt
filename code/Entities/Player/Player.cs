namespace SpookyJam2022;

public partial class Player : BasePlayer
{
	private static Vector3 mins = new Vector3( -16f, -16f, 0f );
	private static Vector3 maxs = new Vector3( 16f, 16f, 72f );
	public static BBox CollisionBox = new BBox( mins, maxs );

	[Net] public bool LockInputs { get; set; } = true;
	[Net] public int HP { get; set; } = 3;

	public BaseInteractable FirstInteractable
	{
		get
		{

			var trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 80f )
				.WorldAndEntities()
				.Ignore( this )
				.Run();

			if ( trace.Entity is BaseInteractable firstInteractable ) return firstInteractable;

			var secondInteractable = FindInSphere( trace.EndPosition, 20f )
				.OfType<BaseInteractable>().MinBy(x => x.Position.Distance( trace.EndPosition ));

			return secondInteractable;
		}
	}

	public BaseInteractable InteractingWith { get; set; }

	public PlayerSpawn CurrentCheckpoint { get; set; }

	public override void Spawn()
	{

		EyeLocalPosition = Vector3.Up * maxs.z;

		SetModel( "models/citizen/citizen.vmdl" ); // Movements are choppy without a model set?
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, mins, maxs );

		EnableDrawing = false; // Singleplayer awww yea
		Tags.Add( "player" );

		Respawn();

	}

	public override void ClientSpawn()
	{

		base.ClientSpawn();

		EnableDrawing = false;

	}

	public override void Respawn()
	{

		CurrentCheckpoint ??= PlayerSpawn.Initial;

		EnableAllCollisions = true;
		EnableDrawing = true;

		Position = CurrentCheckpoint.Position;
		Rotation = CurrentCheckpoint.Rotation;

		ResetInterpolation();
	}

	public void Initial()
	{

		CurrentCheckpoint = PlayerSpawn.Initial;

		EnableAllCollisions = true;
		EnableDrawing = true;

		Position = CurrentCheckpoint.Position;
		Rotation = CurrentCheckpoint.Rotation;

		ResetInterpolation();
	}

	public TimeSince LastInteraction = 0;

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		
		if ( Game.IsClient ) return;
		
		if ( Input.Pressed( InputButton.Use ) )
		{

			if ( LastInteraction >= 0.5f )
			{

				if ( InteractingWith != null )
				{

					InteractingWith.Interact( this );
					ShakeCamera();

				}
				else
				{

					var availableInteractable = FirstInteractable;

					availableInteractable?.Interact( this );
					ShakeCamera();

				}

				LastInteraction = 0;

			}

		}

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
		{

			if ( LastInteraction >= 0.5f )
			{

				if ( Holding != null )
				{

					LastInteraction = 0;
					Holding.Use( this );

				}

			}

		}

		if ( HP > 0 && !LockInputs )
		{

			var onceEvery = 180f * Math.Max( 1, Velocity.Length / 80 );

			if ( Time.Tick % onceEvery == 0 )
			{

				PlaySound( "sounds/misc/breathe_in.sound" );

			}

			if ( ( Time.Tick + onceEvery / 2 ) % onceEvery == 0 )
			{

				PlaySound( "sounds/misc/breathe_out.sound" );

			}

		}

	}

	[ClientRpc]
	public void ShakeCamera()
	{

		Event.Run( "ScreenShake", 0.10f, 3f );

	}

	[Event( "BeginGame" )]
	public void StartGame()
	{

		if ( Game.IsClient ) return;

		LockInputs = false;
		OverrideCamera = null;
		ScriptedEvent = false;

	}
	public override void BuildInput()
	{

		if ( LockInputs ) return;

		InputDirection = Input.AnalogMove;
		InputLook = Input.AnalogLook;

	}
}
