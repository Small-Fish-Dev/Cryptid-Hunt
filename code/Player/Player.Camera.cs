using Sandbox.Services;

namespace CryptidHunt;

public partial class Player
{
	[Property]
	public Vector3 CameraOffset { get; set; } = new Vector3( 0f, 0f, 64f );
	public TimeUntil CameraShake { get; set; } = 0f;

	public Vector3 CameraPosition;
	public Rotation CameraRotation;

	public float ShakeIntensity { get; set; } = 10f;
	public Vector2 _lastShake;

	float _walkBob;
	Vector3 _cameraOffset;

	public void SetupCamera()
	{
		var speed = Controller.Velocity.WithZ( 0f ).Length / 220f;

		if ( Controller.GroundObject.IsValid() )
			_walkBob += Time.Delta * speed * 20f;

		var sideOffset = MathF.Sin( _walkBob * 0.5f ) * speed * -3f;
		var upOffset = MathF.Sin( _walkBob ) * 0.75f * -1.5f;

		if ( CameraShake > 0 )
			_lastShake = new Vector2( (Noise.Perlin( Time.Now * ShakeIntensity * 20f, 18924 ) * 2 - 1) * ShakeIntensity, (Noise.Perlin( Time.Now * ShakeIntensity * 20f, 9124 ) * 2 - 1) * ShakeIntensity );
		else
			_lastShake = Vector2.Lerp( _lastShake, 0f, Time.Delta * ShakeIntensity );

		sideOffset += _lastShake.x;
		upOffset += _lastShake.y;

		CameraOffset = _cameraOffset + Camera.WorldRotation.Left * sideOffset + WorldRotation.Up * upOffset;

		if ( !LockInputs )
		{
			CameraPosition = WorldTransform.PointToWorld( CameraOffset );
			CameraRotation = Controller.EyeAngles;
		}
		else
			CameraPosition += Camera.WorldRotation.Left * sideOffset + WorldRotation.Up * upOffset;

		Camera.WorldPosition = CameraPosition;
		Camera.WorldRotation = CameraRotation;
	}

	public void AddCameraShake( float duration, float intensity )
	{
		CameraShake = duration;
		ShakeIntensity = intensity;
	}
}
