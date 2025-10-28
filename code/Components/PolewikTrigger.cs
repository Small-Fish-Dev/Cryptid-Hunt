namespace CryptidHunt;

public sealed class PolewikTrigger : Component, Component.ITriggerListener
{
	public bool Activated { get; set; } = false;

	public PolewikTrigger() { }

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Parent.Components.TryGet<Player>( out var _, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		var polewick = Scene.Components.Get<Polewik>( FindMode.EverythingInSelfAndDescendants );
		if ( polewick.IsValid() && polewick.GameObject.Enabled && polewick.Alive )
		{
			polewick.CurrentState = PolewikState.Yell;
		}
	}
}
