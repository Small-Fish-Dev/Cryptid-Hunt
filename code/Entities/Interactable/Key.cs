namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/key.vmdl" )]
[Display( Name = "Key", GroupName = "Items", Description = "To open locked chest" )]
public partial class Key : BaseInteractable
{

	public override string ModelPath => "models/items/key.vmdl";
	public override string UseDescription => "Take Key";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Interact( Player player )
	{

		base.Interact( player );

	}
	public override void Use( Player player )
	{

		if ( player.FirstInteractable is LockedChest chest )
		{

			new Shotgun()
			{
				Position = chest.Position,
				Rotation = chest.Rotation
			};

			chest.Delete();

			player.ChangeHolding( null );
			Delete();

		}

	}


}
