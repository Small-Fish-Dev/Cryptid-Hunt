namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_crowbar.vmdl" )]
[Display( Name = "Crowbar", GroupName = "Items", Description = "To open door later on" )]
public partial class Crowbar : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_crowbar.vmdl";
	public override string UseDescription => "Take Crowbar";
	public override Vector3 PromptOffset3D => new Vector3( 0f, -10f );
	public override Vector2 PromptOffset2D => new Vector2( 0f, 0f );

	public override void Interact( Player player )
	{


	}


}
