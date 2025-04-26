namespace CryptidHunt;

public partial class ScriptedEventCamera : Component
{
	[Property]
	public GameObject Target { get; set; }

	[Property]
	public bool DoNotLerp { get; set; } = false;

	[Property]
	public Vector3 LocalTargetOffset { get; set; } = new Vector3( 85f, 0f, 72f );

	protected override void OnUpdate()
	{
		if ( Target.IsValid() )
		{
			var renderer = Target.GetComponent<SkinnedModelRenderer>();
			var cameraTransform = renderer?.SceneModel.GetBoneWorldTransform( "camera" ) ?? (Transform)default;

			WorldPosition = Target.WorldTransform.PointToWorld( LocalTargetOffset );

			if ( cameraTransform == (Transform)default )
				WorldRotation = Rotation.LookAt( Vector3.Direction( WorldPosition, cameraTransform.Position ) );
			else
				WorldRotation = Rotation.LookAt( Vector3.Direction( WorldPosition, Target.WorldPosition + Vector3.Up * 36f ) );
		}
	}
}
