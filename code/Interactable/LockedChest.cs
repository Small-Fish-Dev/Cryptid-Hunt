namespace CryptidHunt;

public partial class LockedChest : Interactable
{
	[Property]
	public ModelRenderer Model { get; set; }
	[Property]
	public GameObject Lock { get; set; }
	public override string InteractDescription => (Player.Instance.Holding is Key || _fading) ? "Open" : "LOCKED";
	public override bool Locked => (Player.Instance.Holding is Key || _fading) ? false : true;

	bool _shaking = false;
	TimeUntil _fadeAway;
	bool _fading = false;
	Vector3 _position;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( _shaking )
		{
			WorldPosition = _position + new Vector3( (Noise.Perlin( Time.Now * 400f, 18924 ) * 2 - 1), (Noise.Perlin( Time.Now * 400f, 9124 ) * 2 - 1), (Noise.Perlin( Time.Now * 400f, 4019 ) * 2 - 1) );
		}

		if ( _fading )
		{
			var alpha = 1f - _fadeAway.Fraction;
			Model.Tint = Model.Tint.WithAlpha( alpha );
			Lock.GetComponent<ModelRenderer>().Tint = Model.Tint.WithAlpha( alpha );
		}
	}

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		Sound.Play( "pickup", WorldPosition );

		if ( player.Holding is Key )
		{
			Sound.Play( "chest_open", WorldPosition );
			player.AddCameraShake( 0.3f, 5f );
			Destruct();
			player.Holding.DestroyGameObject();
			player.Holding = null;
		}
		else
		{
			Sound.Play( "chest_locked", WorldPosition );
			player.AddCameraShake( 0.3f, 8f );
		}
	}

	public async void Destruct()
	{
		_shaking = true;
		_position = WorldPosition;
		await Task.DelaySeconds( 1f );
		_shaking = false;
		Lock.Components.Create<Rigidbody>().ApplyForce( Vector3.Up * 500f );
		await Task.DelaySeconds( 1f );
		_fadeAway = 2f;
		_fading = true;
		await Task.DelaySeconds( 2f );
		DestroyGameObject();
	}
}
