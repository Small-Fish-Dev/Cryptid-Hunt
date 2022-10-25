namespace SpookyJam2022;

public partial class Player : AnimatedEntity
{
	private static Vector3 mins = new Vector3( -16f, -16f, 0f );
	private static Vector3 maxs = new Vector3( 16f, 16f, 72f );
	public static BBox CollisionBox = new BBox( mins, maxs );

	[Net] public bool LockInputs { get; set; } = false;
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
				.OfType<BaseInteractable>()
				.OrderBy( x => x.Position.Distance( trace.EndPosition ) )
				.FirstOrDefault();

			if ( secondInteractable != null ) return secondInteractable;

			return null;

		}
	}

	public BaseInteractable InteractingWith { get; set; }

	[Net, Predicted] public PawnController Controller { get; set; }
	public PlayerSpawn CurrentCheckpoint { get; set; }

	public override void Spawn()
	{

		EyeLocalPosition = Vector3.Up * maxs.z;
		Controller ??= new PlayerController();

		Tags.Add( "player" );

		SetModel( "models/citizen/citizen.vmdl" ); // Movements are choppy without a model set?
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, mins, maxs );

		EnableDrawing = false; // Singleplayer awww yea

		Respawn();

	}

	public override void ClientSpawn()
	{

		base.ClientSpawn();

		EnableDrawing = false;

	}

	public void Respawn()
	{

		CurrentCheckpoint ??= PlayerSpawn.Initial;

		EnableAllCollisions = true;
		EnableDrawing = true;

		Position = CurrentCheckpoint.Position;
		Rotation = CurrentCheckpoint.Rotation;

		ResetInterpolation();
	}

	public TimeSince LastInteraction = 0;

	public override void Simulate( Client cl )
	{

		Controller?.Simulate( cl, this, null );

		if ( Host.IsClient ) return;
		
		if ( Input.Pressed( InputButton.Use ) )
		{

			if ( LastInteraction >= 0.5f )
			{

				if ( InteractingWith != null )
				{

					InteractingWith.Interact( this );

				}
				else
				{

					var availableInteractable = FirstInteractable;

					if ( availableInteractable != null )
					{

						availableInteractable.Interact( this );

					}

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

					Holding.Use( this );

					LastInteraction = 0;

				}

			}

		}

	}

	public override void FrameSimulate( Client cl )
	{
		Controller?.FrameSimulate( cl, this, null );
		EyeRotation = Input.Rotation;
	}

	public override void BuildInput( InputBuilder input )
	{

		if ( LockInputs ) return;

		input.ViewAngles += input.AnalogLook;
		input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
		input.InputDirection = input.AnalogMove;

	}
}
