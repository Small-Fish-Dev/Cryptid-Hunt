namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_locked_chest.vmdl" )]
[Display( Name = "Locked Chest", GroupName = "Items", Description = "To be opened with key" )]
public partial class LockedChest : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_locked_chest.vmdl";
	public override string UseDescription => "Locked Chest";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );
	public override bool Locked { get; set; } = true;

	public override void Interact( Player player )
	{

		if ( !Locked )
		{


			player.Holding.Delete();
			player.ChangeHolding( null );

			new Shotgun()
			{
				Position = Position,
				Rotation = Rotation
			};

			Delete();

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
