namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/editor/camera.vmdl" )]
[Display( Name = "Scripted Event Camera", GroupName = "Cinematic", Description = "Set this up where you want the camera to be during scripted events" )]
public partial class ScriptedEventCamera : Entity
{

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}
}
