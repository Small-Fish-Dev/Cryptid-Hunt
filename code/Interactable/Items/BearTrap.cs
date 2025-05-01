namespace CryptidHunt;

public sealed class BearTrap : Item
{
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( Player.Instance.Holding != this ) return;

		var trace = Player.Instance.InteractTrace;
		var rotation = Rotation.LookAt( trace.Hit ? trace.Normal : Vector3.Up ) * Rotation.FromPitch( 90f );

		DebugOverlay.Model( Model.Model, trace.Hit ? Color.Green.WithAlpha( 0.7f ) : Color.Red.WithAlpha( 0.7f ), Time.Delta, new Transform( trace.EndPosition, rotation ) );
	}
	public override void Attack( Player player )
	{
		var trace = Player.Instance.InteractTrace;

		if ( !trace.Hit ) return;

		GameObject.Enabled = true;
		WorldPosition = trace.EndPosition;
		WorldRotation = Rotation.LookAt( trace.Normal ) * Rotation.FromPitch( 90f );
		player.Remove( this );
	}
}
