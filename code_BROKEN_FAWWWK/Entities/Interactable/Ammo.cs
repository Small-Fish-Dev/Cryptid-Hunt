namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/ammo.vmdl" )]
[Display( Name = "Ammo Box", GroupName = "Items", Description = "Ammo Box for shotgun" )]
[Item( "shotgun_ammo" )]
public partial class Ammo : BaseInteractable
{

	public override string ModelPath => "models/items/ammo.vmdl";
	public override string UseDescription => "Take Ammo";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Spawn()
	{
		base.Spawn();

		Amount = Game.Random.Int( 3, 5 );
	}

	public override void Interact( Player player )
	{

		base.Interact( player );

	}


}
