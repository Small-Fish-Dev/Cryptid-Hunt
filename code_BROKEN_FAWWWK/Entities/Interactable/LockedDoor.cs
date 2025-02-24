namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/stuck_door.vmdl" )]
[Display( Name = "Locked Door", GroupName = "Items", Description = "To be opened with crowbar" )]
public partial class LockedDoor : BaseInteractable
{

	public override string ModelPath => "models/stuck_door.vmdl";
	public override string UseDescription => "Blocked Door";
	public override Vector3 PromptOffset3D => new Vector3( 0f, 0f, 45f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );
	public override bool Locked { get; set; } = true;

	public override void Interact( Player player )
	{

		if ( !Locked )
		{

			player.Holding.Delete();
			player.ChangeHolding( null );

			Sound.FromWorld( "sounds/items/metal_door_creak.sound", Position ).SetVolume( 2 );

			var door = new ModelEntity();
			door.Position = Position;
			door.Rotation = Rotation;
			door.Scale = Scale;
			door.UsePhysicsCollision = true;
			door.SetModel( GetModelName() );
			door.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			door.EnableAllCollisions = true;
			door.Velocity = door.Rotation.Up * 20f + door.Rotation.Left * 50f;
			door.DeleteAsync( 2f );

			Delete();

		}

	}

	[Event.Tick] //this is real bad xD
	void checkForCrowbar()
	{

		foreach ( var player in Entity.All.OfType<Player>() )
		{

			if ( player.FirstInteractable == this )
			{

				if ( player.Holding is Crowbar )
				{

					Locked = false;
					return;

				}

			}

			Locked = true;

		}

	}


}
