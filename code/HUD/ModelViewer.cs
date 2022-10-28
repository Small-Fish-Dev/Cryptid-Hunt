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
		var light = new SceneLight( world, Vector3.Forward * 100f + Vector3.Up * 20f, 100f, Color.White * 0.5f );

		pitch = obj.Rotation.Pitch();
		yaw = obj.Rotation.Yaw();

		Camera.World = world;
		Camera.Position = Vector3.Forward * 100f;
		Camera.Rotation = Rotation.From( 0, 180, 0 );
		Camera.FieldOfView = 60f;

		AddEventListener( "onmousedown", () => shouldMove = true );
		AddEventListener( "onmouseup", () => shouldMove = false );
	}

	[Event.Frame]
	private void onFrame()
	{
		if ( shouldMove )
		{
			var deltaPos = Mouse.Position - oldPos;
			pitch = (pitch + deltaPos.y) % 360;
			yaw = (yaw + deltaPos.x) % 360;

			obj.Rotation = Rotation.From( pitch, yaw, 0 );
		}

		obj?.Update( Time.Delta );
		oldPos = Mouse.Position;
	}
}
