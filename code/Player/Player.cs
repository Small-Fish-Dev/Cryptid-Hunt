using static Sandbox.Event;

namespace SpookyJam2022;

public partial class Player : AnimatedEntity
{
	private static Vector3 mins = new Vector3( -16f, -16f, 0f );
	private static Vector3 maxs = new Vector3( 16f, 16f, 72f );
	public static BBox CollisionBox = new BBox( mins, maxs );

	[Net, Predicted] public PawnController Controller { get; set; }
	public PlayerSpawn CurrentCheckpoint { get; set; }

	public override void Spawn()
	{

		EyeLocalPosition = Vector3.Up * maxs.z;
		Controller ??= new PlayerController();

		SetModel( "models/citizen/citizen.vmdl" ); // Movements are choppy without a model set?
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, mins, maxs );

		EnableDrawing = false; // Singleplayer awww yea

		Respawn();
	}

	public override void ClientSpawn()
	{

		base.ClientSpawn();

		EnableDrawing = false;

		#region TESTING
		var viewModel = new PlayerViewModel();
		viewModel.Position = Position;
		viewModel.Owner = this;
		viewModel.EnableViewmodelRendering = true;
		viewModel.SetModel( "models/first_person/first_person_arms.vmdl" );
		#endregion

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

	bool sillyNoteToggle = false; // REMOVE THIS WHEN IMPLEMENTED PHYSICAL NOTES

	public override void Simulate( Client cl )
	{
		Controller?.Simulate( cl, this, null );

		if ( Host.IsServer ) return;
		
		if ( Input.Pressed( InputButton.Use ) )
		{

			if ( sillyNoteToggle )
			{

				Event.Run( "HideNotePage" );

			}
			else
			{

				Event.Run( "CreateNotePage", "Help!!!\nI am going to die here\nTHE MONSTER IS COMING\nIt comes from that damn Ape Tavern\nOH NO THEY FOUND ME!\n\n\nHEEELP\nOH NOOOO\nAAACK\n\n\nugh\noof\nack\nWAAAAA!!!", true );

			}

			sillyNoteToggle = !sillyNoteToggle;

		}

	}

	public override void FrameSimulate( Client cl )
	{
		Controller?.FrameSimulate( cl, this, null );
		EyePosition = Position + EyeLocalPosition;
		EyeRotation = Input.Rotation;
	}

	public override void BuildInput( InputBuilder input )
	{
		input.ViewAngles += input.AnalogLook;
		input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
		input.InputDirection = input.AnalogMove;
	}
}
