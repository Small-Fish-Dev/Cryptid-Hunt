namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_beartrap.vmdl" )]
[Display( Name = "Bear Trap", GroupName = "Items", Description = "Bear trap to trap bears or cryptids" )]
public partial class BearTrap : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_beartrap.vmdl";
	public override string UseDescription => "Take Bear Trap";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Interact( Player player )
	{

		base.Interact( player );

	}
	public override void Use( Player player )
	{

		var trace = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 130f )
			.Ignore( player )
			.Run();


		if ( trace.Hit && trace.Normal.z >= 0.4f )
		{

			Parent = null;
			EnableAllCollisions = true;
			Position = trace.HitPosition;
			Rotation = Rotation.LookAt( trace.Normal ).RotateAroundAxis( Vector3.Right, -90f );
			EnableDrawing = true;
			EnableShadowCasting = true;

			player.Holding = null;

		}

	}

}
