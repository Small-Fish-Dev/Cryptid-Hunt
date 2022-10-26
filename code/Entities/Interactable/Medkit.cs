namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/medkit.vmdl" )]
[Display( Name = "Medkit", GroupName = "Items", Description = "Heals the player to full hp" )]
public partial class Medkit : BaseInteractable
{

	public override string ModelPath => "models/items/medkit.vmdl";
	public override string UseDescription => "Take Medkit";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( -80f, 0f );
	public override Rotation OffsetRotation => Rotation.From( new Angles( 0f, -70f, 0f ) );
	public override Vector3 OffsetPosition => new Vector3( 5f, -5f, -5f );

	public override void Interact( Player player )
	{


		base.Interact( player );

	}
	public override void Use( Player player )
	{

		player.HP = 3;

		player.ChangeHolding( null );
		Delete();

	}

}
