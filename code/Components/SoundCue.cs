namespace CryptidHunt;

public partial class MySoundCue : Component, Component.ITriggerListener
{
	[Property]
	public SoundEvent Sound { get; set; }
	public bool Activated { get; set; } = false;

	public MySoundCue() { }

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Parent.Components.TryGet<Player>( out var player, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		GameObject.PlaySound( Sound, GameObject.WorldPosition );
	}
}
