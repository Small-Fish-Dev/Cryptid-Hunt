namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/chest.vmdl" )]
[Display( Name = "Locked Chest", GroupName = "Items", Description = "To be opened with key" )]
public partial class LockedChest : BaseInteractable
{

	public override string ModelPath => "models/items/chest.vmdl";
	public override string UseDescription => "Locked Chest";
	public override Vector3 PromptOffset3D => new Vector3( 10f, 0f, 15f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 60f );
	public override bool Locked { get; set; } = true;

	[Net] public ModelEntity LockModel { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		LockModel = new ModelEntity( "models/items/lock.vmdl" );
		LockModel.Position = GetAttachment( "lock" )?.Position ?? Position;
		LockModel.Rotation = GetAttachment( "lock" )?.Rotation ?? Rotation;
		LockModel.EnableAllCollisions = false;

	}

	public override void Interact( Player player )
	{

		if ( !Locked )
		{


			player.Holding.Delete();
			player.ChangeHolding( null );

			var dynamicLock = new ModelEntity();
			dynamicLock.Position = LockModel.Position;
			dynamicLock.Rotation = LockModel.Rotation;
			dynamicLock.Scale = LockModel.Scale;
			dynamicLock.UsePhysicsCollision = true;
			dynamicLock.SetModel( LockModel.GetModelName() );
			dynamicLock.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			dynamicLock.EnableAllCollisions = true;

			CurrentSequence.Name = "top_open";

			LockModel.Delete();

			PlaySound( "sounds/items/chest_open.sound" );


			var shotgun = new Shotgun();
			shotgun.Position = Position;
			shotgun.Rotation = Rotation.FromYaw( 45f );
			shotgun.EnableAllCollisions = false;
			shotgun.EnableDrawing = false;

			GameTask.RunInThreadAsync( async () =>
			{

				await GameTask.DelaySeconds( 1f );

				shotgun.EnableAllCollisions = true;
				shotgun.EnableDrawing = true;

				dynamicLock.Delete();
				Delete();



			} );

		}
		else
		{

			PlaySound( "sounds/items/chest_locked.sound" );

		}

	}

	[Event.Tick] //this is real bad xD
	void checkForKey()
	{

		foreach ( var player in Entity.All.OfType<Player>() )
		{

			if ( player.FirstInteractable == this )
			{

				if ( player.Holding is Key )
				{

					Locked = false;
					return;

				}

			}

			Locked = true;

		}

	}


}
