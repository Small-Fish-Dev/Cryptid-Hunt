namespace SpookyJam2022;

public class ModelViewer : ScenePanel
{
	SceneModel obj;
	Vector2 oldPos;
	bool shouldMove = false;

	float pitch;
	float yaw;

	public ModelViewer( Model mdl, Transform transform )
	{
		var world = new SceneWorld();
		obj = new SceneModel(
			world,
			mdl,
			transform );


		var lightWarm = new SceneSpotLight( world, Vector3.Forward * 100f + Vector3.Up * 30f + Vector3.Left * 100f, new Color( 1f, 0.95f, 0.8f ) * 15f );
		lightWarm.Rotation = Rotation.LookAt( -lightWarm.Position );
		lightWarm.Radius = 200f;
		lightWarm.ConeInner = 90f;
		lightWarm.ConeOuter = 90f;

		var lightBlue = new SceneSpotLight( world, Vector3.Backward * 80f + Vector3.Down * 30f + Vector3.Right * 150f, new Color( 0.4f, 0.4f, 1f ) * 20f );
		lightBlue.Rotation = Rotation.LookAt( -lightBlue.Position );
		lightBlue.Radius = 200f;
		lightBlue.ConeInner = 90f;
		lightBlue.ConeOuter = 90f;

		var lightAmbient = new SceneSpotLight( world, Vector3.Forward * 100f, new Color( 1f, 1f, 1f ) * 1f );
		lightBlue.Rotation = Rotation.LookAt( -lightBlue.Position );
		lightBlue.Radius = 200f;
		lightBlue.ConeInner = 90f;
		lightBlue.ConeOuter = 90f;

		pitch = obj.Rotation.Pitch();
		yaw = obj.Rotation.Yaw();

		Camera.World = world;
		Camera.Position = Vector3.Forward * 100f;
		Camera.Rotation = Rotation.From( 0, 180, 0 );
		Camera.FieldOfView = 60f;

		AddEventListener( "onmousedown", () => shouldMove = true );
		AddEventListener( "onmouseup", () => shouldMove = false );
	}

	[Event.Client.Frame]
	private void onFrame()
	{
		if ( shouldMove )
		{

			if ( oldPos == 0f )
			{

				oldPos = Mouse.Position;

			}

			var deltaPos = Mouse.Position - oldPos;
			pitch = (pitch + deltaPos.y) % 360;
			yaw = (yaw - deltaPos.x ) % 360;
			
			Camera.Rotation = Rotation.From( pitch, (yaw + 180) % 360, 0 );
			Camera.Position = Camera.Rotation.Forward * -100f;


		}

		obj?.Update( Time.Delta );
		oldPos = Mouse.Position;
	}

}
