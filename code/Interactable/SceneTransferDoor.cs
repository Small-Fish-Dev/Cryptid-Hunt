using Sandbox;

namespace CryptidHunt;

public partial class SceneTransferDoor : Interactable
{
	[Property]
	public ModelRenderer Model { get; set; }
	[Property]
	public SpawnPoint SpawnPoint { get; set; }

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
			NextScene();
			Sound.Play( "door_open", WorldPosition );
			Open = true;
			_degrees = 25f;
		}

		player.AddCameraShake( 0.3f, 8f );
	}

	public async void NextScene()
	{
		GameUI.BlackScreen();
		Player.Instance.LockInputs = true;
		await Task.DelaySeconds( 2.5f );
		Player.Instance.LockInputs = false;
		Player.Instance.WorldPosition = SpawnPoint.WorldPosition;
		Player.Instance.Controller.EyeAngles = SpawnPoint.WorldRotation;
		await Task.DelaySeconds( 4f );

		Player.Instance.SetFlashLight( true, true );
	}
}
