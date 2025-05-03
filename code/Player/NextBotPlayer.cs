namespace CryptidHunt;

public partial class NextBotPlayer : Component
{
	[Property]
	public Computer Computer { get; set; }

	[Property]
	public PlayerController Controller { get; set; }

	[Property]
	public GameObject Camera { get; set; }

	[Property]
	public Rigidbody CameraBody { get; set; }

	public bool Alive { get; set; } = false;

	protected override void OnStart()
	{
		base.OnStart();

		Respawn();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !Controller.IsValid() ) return;
		if ( !Camera.IsValid() ) return;

		if ( Computer.Started && Alive )
		{
			Controller.UseInputControls = Computer.Playing;
			Camera.WorldRotation = Controller.EyeAngles;
			Camera.LocalPosition = Vector3.Up * 64f;
		}
		else
		{
			Controller.UseInputControls = false;
			Controller.WishVelocity = 0f;
		}
	}

	public async void Die()
	{
		if ( !Alive ) return;
		Alive = false;
		Camera.SetParent( null );
		CameraBody.Enabled = true;

		await Task.DelaySeconds( 4f );
		Respawn();
	}

	public void Respawn()
	{
		if ( Alive ) return;
		Alive = true;
		Camera.SetParent( GameObject );
		CameraBody.Enabled = false;

		var spawnPoints = Scene.Components.GetAll<SpawnPoint>().Where( x => x.Tags.Has( "nextbot_player" ) );
		var spawnPoint = Game.Random.FromList( spawnPoints.ToList() );
		WorldTransform = spawnPoint.WorldTransform;
	}
}
