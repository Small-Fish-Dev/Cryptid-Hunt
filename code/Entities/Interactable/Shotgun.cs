namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_shotgun.vmdl" )]
[Display( Name = "Shotgun", GroupName = "Items", Description = "Shotgun shoot pew" )]
public partial class Shotgun : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_shotgun.vmdl";
	public override string UseDescription => "Take Shotgun";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Interact( Player player )
	{


	}

}
