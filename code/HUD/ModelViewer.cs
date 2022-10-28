namespace SpookyJam2022;

public class ModelViewer : ScenePanel
{
	SceneModel obj;
	Vector2 oldPos;
	bool shouldMove = false;

	public ModelViewer( Model mdl, Transform transform )
	{
		var world = new SceneWorld();
		obj = new SceneModel(
			world,
			mdl,
			transform );
		var light = new SceneLight( world, Vector3.Forward * 100f + Vector3.Up * 20f, 100f, Color.White * 0.5f );

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
			var pitch = obj.Rotation.Pitch() + deltaPos.y;
			var yaw = obj.Rotation.Yaw() + deltaPos.x;
			Log.Error( (pitch, yaw) );
			obj.Rotation = Rotation.From( pitch, yaw, 0 );
		}

		obj?.Update( Time.Delta );
		oldPos = Mouse.Position;
	}
}
