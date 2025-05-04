using System;

namespace CryptidHunt;

public sealed class BearTrap : Item, Component.ITriggerListener
{
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Player.Instance?.Holding != this ) return;

		var trace = Player.Instance.InteractTrace;
		var position = trace.EndPosition - trace.Normal * 5f;
		var rotation = Rotation.LookAt( trace.Hit ? trace.Normal : Vector3.Up ) * Rotation.FromPitch( 90f );
		var canPlace = trace.Hit && Vector3.Dot( trace.Normal, Vector3.Up ) > 0.8f;

		DebugOverlay.Model( Model.Model, canPlace ? Color.Green.WithAlpha( 0.7f ) : Color.Red.WithAlpha( 0.7f ), Time.Delta, new Transform( position, rotation ) );
	}
	public override void Attack( Player player )
	{
		var trace = Player.Instance.InteractTrace;

		if ( !trace.Hit || Vector3.Dot( trace.Normal, Vector3.Up ) <= 0.8f ) return;

		GameObject.Enabled = true;
		WorldPosition = trace.EndPosition - trace.Normal * 5f;
		WorldRotation = Rotation.LookAt( trace.Normal ) * Rotation.FromPitch( 90f );
		player.Remove( this );
		Sound.Play( "beartrap_set", WorldPosition );
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active ) return;
		if ( !other.Components.TryGet<Polewik>( out var polewik, FindMode.EnabledInSelf ) ) return;

		polewik.HP -= 10f;
		Sound.Play( "beartrap_trigger", WorldPosition );
		GameObject.Destroy();
	}
}
