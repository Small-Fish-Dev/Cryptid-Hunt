namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_ammo.vmdl" )]
[Display( Name = "Ammo Box", GroupName = "Items", Description = "Ammo Box for shotgun" )]
public partial class Ammo : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_ammo.vmdl";
	public override string UseDescription => "Take Ammo";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Interact( Player player )
	{


	}


}
