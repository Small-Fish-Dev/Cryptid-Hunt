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
		var firstAmmo = player.Items.FirstOrDefault( x => x.Title == "Ammo" && x.Amount > 0 ); // LOL
		if ( firstAmmo == null ) return;
		var ammoLeft = firstAmmo?.Amount ?? 0;
		if ( ammoLeft <= 0 ) return;
		firstAmmo.Amount--;
		if ( firstAmmo.Amount <= 0 )
			firstAmmo.DestroyGameObject();

		var damageDealt = 0f;
		Polewik polewik = null;
		for ( int i = 0; i < 10; i++ )
		{
			var rotation = player.Camera.WorldRotation;
			rotation *= Rotation.FromYaw( Game.Random.Float( -9f, 9f ) );
			rotation *= Rotation.FromPitch( Game.Random.Float( -9f, 9f ) );

			var shootTrace = Scene.Trace.Ray( player.Camera.WorldPosition, player.Camera.WorldPosition + rotation.Forward * 1500f )
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

			if ( !shootTrace.GameObject.Components.TryGet<Polewik>( out polewik, FindMode.EverythingInSelfAndDescendants ) ) continue;

			damageDealt += 100f; // TODO: 1
		}

		if ( polewik != null )
			polewik.HP -= damageDealt;

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
		_nextBulletCullCheck = 1f;

		foreach ( var bullet in _bulletHoles.ToList() )
		{
			if ( !bullet.Value ) continue;
			bullet.Key.Destroy();
		}
	}
}
