namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/bedroom/cardboard/cardboard.vmdl" )]
[Display( Name = "Cardboard", GroupName = "Items", Description = "Can be removed at cutscene haha" )]
public partial class Cardboard : BaseInteractable
{

	public override string ModelPath => "models/bedroom/cardboard/cardboard.vmdl";
	public override string UseDescription => "Remove";
	public override Vector3 PromptOffset3D => new Vector3( 0f, 0f, 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Spawn()
	{

		base.Spawn();
		EnableAllCollisions = false;
		EnableDrawing = false;

	}

	[Event( "StartCutscene" )]
	public void Cutscene()
	{

		EnableAllCollisions = true;
		EnableDrawing = true;

	}

	public override void Interact( Player player )
	{

		Event.Run( "RemovedCardboard" );
		Sound.FromScreen( "sounds/scary/slam_flute.sound" ).SetVolume( 2 );
		Delete();

	}

}
