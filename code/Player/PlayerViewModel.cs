namespace SpookyJam2022;

public class PlayerViewModel : BaseViewModel
{
	float walkBob = 0f;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{

		base.PostCameraSetup( ref camSetup );

		if ( !Local.Pawn.IsValid() ) { return; }

		Entity ply = Local.Pawn;

		Position = ply.EyePosition - ply.EyeRotation.Forward * 20;
		Rotation = ply.EyeRotation;

		var speed = ply.Velocity.Length / 300f;
		var left = camSetup.Rotation.Left;
		var up = camSetup.Rotation.Up;

		if ( ply.GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Position += up * MathF.Sin( walkBob ) * speed * -1;
		Position += left * MathF.Sin( walkBob * 0.6f ) * speed * -0.5f * Math.Max( ply.Rotation.Forward.Dot( ply.Velocity ), 0 ); // To make the bobbing less noticeble when not walking forward

		camSetup.ViewModel.FieldOfView = 70f;

	}
}
