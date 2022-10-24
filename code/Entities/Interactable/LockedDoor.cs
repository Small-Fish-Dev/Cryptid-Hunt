namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_locked_door.vmdl" )]
[Display( Name = "Locked Door", GroupName = "Items", Description = "To be opened with crowbar" )]
public partial class LockedDoor : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_locked_door.vmdl";
	public override string UseDescription => "Blocked Door";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );
	public override bool Locked { get; set; } = true;

	public override void Interact( Player player )
	{


	}


}
