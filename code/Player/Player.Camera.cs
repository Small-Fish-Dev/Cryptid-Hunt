using Sandbox.Services;

namespace CryptidHunt;

public partial class Player
{
	[Property]
	public Vector3 CameraOffset { get; set; } = new Vector3( 0f, 0f, 64f );

	public Vector3 CameraPosition;
	public Rotation CameraRotation;
	private TimeUntil _cameraShake = 0f;
	private float _shakeIntensity = 10f;
	private Vector2 _lastShake;


	float _walkBob;
	Vector3 _cameraOffset;

	public void SetupCamera()
	{
		if ( Holding.IsValid() )
			Holding.Model.Enabled = !LockInputs;

		var speed = Controller.Velocity.WithZ( 0f ).Length / 220f;

		if ( Controller.GroundObject.IsValid() )
			_walkBob += Time.Delta * speed * 20f;

		var sideOffset = MathF.Sin( _walkBob * 0.5f ) * speed * -3f;
		var upOffset = MathF.Sin( _walkBob ) * 0.75f * -1.5f;

		if ( _cameraShake > 0 )
			_lastShake = new Vector2( (Noise.Perlin( Time.Now * _shakeIntensity * 20f, 18924 ) * 2 - 1) * _shakeIntensity, (Noise.Perlin( Time.Now * _shakeIntensity * 20f, 9124 ) * 2 - 1) * _shakeIntensity );
		else
			_lastShake = Vector2.Lerp( _lastShake, 0f, Time.Delta * _shakeIntensity );

		sideOffset += _lastShake.x;
		upOffset += _lastShake.y;

		if ( !LockInputs )
			CameraOffset = _cameraOffset + Vector3.Left * sideOffset + Vector3.Up * upOffset;
		else
			CameraOffset = Vector3.Left * sideOffset + Vector3.Up * upOffset;

		if ( !LockInputs )
		{
			CameraPosition = WorldTransform.PointToWorld( CameraOffset );
			CameraRotation = Controller.EyeAngles;

			Camera.WorldPosition = CameraPosition;
			Camera.WorldRotation = CameraRotation;
		}
		else
		{
			Camera.WorldPosition = CameraPosition + CameraOffset * Camera.WorldRotation;
			Camera.WorldRotation = CameraRotation;
		}
	}

	public void AddCameraShake( float duration, float intensity )
	{
		_cameraShake = duration;
		_shakeIntensity = intensity;
	}
}
