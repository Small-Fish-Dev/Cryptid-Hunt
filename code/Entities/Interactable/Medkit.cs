namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_medkit.vmdl" )]
[Display( Name = "Medkit", GroupName = "Items", Description = "Heals the player to full hp" )]
public partial class Medkit : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_medkit.vmdl";
	public override string UseDescription => "Take Medkit";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Interact( Player player )
	{


	}

}
