namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_cinder.vmdl" )]
[Display( Name = "Brick", GroupName = "Items", Description = "It disappear" )]
public partial class Brick : ModelEntity
{

	public override void Spawn()
	{

		base.Spawn();
		EnableAllCollisions = true;
		EnableDrawing = true;

	}

	[Event( "StartCutscene" )]
	public void Cutscene()
	{

		EnableAllCollisions = false;
		EnableDrawing = false;

	}

}
