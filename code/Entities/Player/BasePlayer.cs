namespace SpookyJam2022;

public abstract partial class BasePlayer : AnimatedEntity
{
	[Net, Predicted] public PawnController Controller { get; set; }
	
	public override void Spawn()
	{
		base.Spawn();
		
		Tags.Add( "player" );
	}

	public abstract void Respawn();

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		
		Controller?.Simulate( cl, this, null );
	}
}
