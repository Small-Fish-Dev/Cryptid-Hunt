namespace CryptidHunt;

public sealed class Shotgun : Item
{
	[Property]
	public GameObject BulletHolePrefab { get; set; }
	[Property]
	public Light Light { get; set; }

	Dictionary<GameObject, TimeUntil> _bulletHoles = new();

	public override void Attack( Player player )
	{
		var firstAmmo = player.Items.FirstOrDefault( x => x.Title == "Ammo" ); // LOL
		var ammoLeft = firstAmmo?.Amount ?? 0;
		if ( ammoLeft <= 0 ) return;
		firstAmmo.Amount--;
		if ( firstAmmo.Amount <= 0 )
			firstAmmo.DestroyGameObject();

		for ( int i = 0; i < 10; i++ )
		{
			var rotation = player.Camera.WorldRotation;
			rotation *= Rotation.FromYaw( Game.Random.Float( -5f, 5f ) );
			rotation *= Rotation.FromPitch( Game.Random.Float( -5f, 5f ) );

			var shootTrace = Scene.Trace.Ray( player.Camera.WorldPosition, player.Camera.WorldPosition + rotation.Forward * 1000f )
				.Radius( 5f )
				.IgnoreGameObjectHierarchy( player.GameObject )
				.Run();

			if ( !shootTrace.Hit ) continue;

			var bullet = BulletHolePrefab.Clone();
			bullet.WorldPosition = shootTrace.EndPosition - shootTrace.Normal * 5f;
			var bulletRotation = Rotation.LookAt( -shootTrace.Normal );
			bulletRotation *= Rotation.FromAxis( bulletRotation.Right, Game.Random.Float( -90f, 90f ) );
			bullet.WorldRotation = bulletRotation;

			_bulletHoles.Add( bullet, 30f );

			if ( !shootTrace.GameObject.IsValid() ) continue;

			bullet.SetParent( shootTrace.GameObject );

			if ( !shootTrace.GameObject.Components.TryGet<Polewik>( out var polewik, FindMode.EverythingInSelfAndDescendants ) ) continue;

			var closestBone = polewik.GameObject.Children.OrderBy( x => x.WorldPosition.Distance( bullet.WorldPosition ) ).FirstOrDefault();

			if ( closestBone.IsValid() )
				bullet.SetParent( closestBone );

			polewik.HP -= 100;
		}

		Player.Instance.NextInteraction = 1f;
		Player.Instance.AddCameraShake( 0.3f, 20f );
		Sound.Play( "shotgun_fire", player.Camera.WorldPosition );

		Task.RunInThreadAsync( async () =>
		{
			Light.Enabled = true;
			await Task.DelaySeconds( 0.05f );
			Light.Enabled = false;
		} );
	}

	TimeUntil _nextBulletCullCheck;

	protected override void OnFixedUpdate()
	{
		if ( !_nextBulletCullCheck ) return;

		foreach ( var bullet in _bulletHoles.ToList() )
		{
			if ( !bullet.Value ) continue;
			bullet.Key.Destroy();
		}
	}
}
