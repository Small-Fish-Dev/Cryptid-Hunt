namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/stuck_door.vmdl" )]
[Display( Name = "Locked Door", GroupName = "Items", Description = "To be opened with crowbar" )]
public partial class LockedDoor : BaseInteractable
{

	public override string ModelPath => "models/stuck_door.vmdl";
	public override string UseDescription => "Blocked Door";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );
	public override bool Locked { get; set; } = true;

	public override void Interact( Player player )
	{

		if ( !Locked )
		{

			player.Holding.Delete();
			player.ChangeHolding( null );

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
