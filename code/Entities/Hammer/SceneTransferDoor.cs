namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/bedroom/door.vmdl" )]
[Display( Name = "Scene Transfer Door", GroupName = "Cinematic", Description = "Player can use this door to get transported to the checkpoint" )]
public partial class SceneTransferDoor : BaseInteractable
{

	public override string ModelPath => "models/bedroom/door.vmdl";
	public override string UseDescription => "Exit";
	public override Vector3 PromptOffset3D => new Vector3( 32f, 0f, -5f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 0f );
	public override bool Locked { get; set; } = true;
	[Net, Property, Description( "The ID of the Checkpoint which the player will be transported" )]
	public int CheckpoindIDTarget { get; set; } = 0;
	Rotation targetRotation;
	[Net] public bool Open { get; set; } = false;

	public override void Interact( Player player )
	{

		if ( Locked ) return;
		if ( Open ) return;

		var target = PlayerSpawn.WithID( CheckpoindIDTarget );

		CryptidHunt.Instance.StartBlackScreen();

		targetRotation = Rotation.RotateAroundAxis( Vector3.Up, -20f );
		Open = true;
		player.LockInputs = true;

		Sound.FromEntity( "sounds/items/door_open.sound", this );

		GameTask.RunInThreadAsync( async () =>
		{

			await Task.DelaySeconds( 2f );

			Sound.FromScreen( "sounds/misc/car_intro.sound", 0.5f, 1f );

			await Task.DelaySeconds( 2.5f );

			player.LockInputs = false;

			foreach ( var ply in Entity.All.OfType<Player>() )
			{

				ply.Position = target.Position;
				ply.Rotation = target.Rotation;

			}

			await Task.DelaySeconds( 2.5f );

			Event.Run( "FlashLight", true, true );

			await Task.DelaySeconds( 1f );

			Sound.FromScreen( "sounds/music/piano_intro.sound" );

		} );

		

	}

	[Event.Tick.Server]
	void openDoor()
	{

		if ( Open )
		{

			Rotation = Rotation.Lerp( Rotation, targetRotation, Time.Delta * 3f );

		}

	}

}
