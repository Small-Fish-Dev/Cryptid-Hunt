namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/editor/camera.vmdl" )]
[Display( Name = "Scripted Event Camera", GroupName = "Cinematic", Description = "Set this up where you want the camera to be during scripted events" )]
public partial class ScriptedEventCamera : Entity
{

	[Net] public Polewik Target { get; set; }
	[Net] public float TransitionSpeed { get; set; } = 1;

	public ScriptedEventCamera() { }

	public ScriptedEventCamera( Polewik target, float transSpeed = 1f )
	{

		Target = target;
		TransitionSpeed = transSpeed;

	}

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	[Event.Tick.Server]
	void calcTarget()
	{

		if ( Target != null )
		{

			Position = Target.Transform.PointToWorld( new Vector3( 70f, 0f, 72f ) );
			Rotation = Rotation.LookAt( Target.GetBoneTransform( Target.GetBoneIndex( "camera" ) ).Position - Position );

		}

	}

}
