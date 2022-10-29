namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/crowbar.vmdl" )]
[Display( Name = "Crowbar", GroupName = "Items", Description = "To open door later on" )]
[Item( "crowbar" )]
public partial class Crowbar : BaseInteractable
{
	
	public override string ModelPath => "models/items/crowbar.vmdl";
	public override string UseDescription => "Take Crowbar";
	public override Vector3 PromptOffset3D => new Vector3( 15f, 0f, 0f );
	public override Vector2 PromptOffset2D => new Vector2( 0f, -30f );
	public override Rotation OffsetRotation => Rotation.From( new Angles( -90f, 100f, 0f) );
	public override Vector3 OffsetPosition => new Vector3( -15f, 0f, 0f );

	public override void Interact( Player player )
	{

		base.Interact( player );

	}

	public override void Use( Player player )
	{

		if ( player.FirstInteractable is LockedDoor door )
		{

			door.Delete();

			player.ChangeHolding( null );
			Delete();

		}

	}

}
