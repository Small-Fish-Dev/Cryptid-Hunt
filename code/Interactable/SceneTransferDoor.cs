namespace CryptidHunt;

public partial class SceneTransferDoor : Interactable
{
	[Property]
	public ModelRenderer Model { get; set; }
	public override string InteractDescription => Locked ? "LOCKED" : "Open";
	public override bool Locked => !GameManager.Instance.ReadInitialNote;
	public bool Open { get; set; } = false;
	float _degrees = 0f;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !Open ) return;

		WorldRotation *= Rotation.FromYaw( -_degrees * Time.Delta );
		_degrees = Math.Max( _degrees - Time.Delta * 25f, 0f );
	}

	public override void Interact( Player player )
	{
		if ( !player.IsValid() ) return;

		if ( Open ) return;

		if ( Locked )
			Sound.Play( "chest_locked", WorldPosition );
		else
		{
			Sound.Play( "door_open", WorldPosition );
			Open = true;
			_degrees = 25f;
		}

		player.AddCameraShake( 0.3f, 8f );
	}
}
