using CryptidHunt;
using Sandbox;

public sealed class NextBot : Component
{
	[Property]
	public NavMeshAgent Agent { get; set; }

	[Property]
	public SpriteRenderer SpriteRenderer { get; set; }

	[Property]
	public NextBotPlayer Target { get; set; }

	[Property]
	public Computer Computer { get; set; }

	[Property]
	public SoundEvent ScarySound { get; set; }


	TimeUntil _nextSound = 0f;
	protected override void OnFixedUpdate()
	{
		var distance = WorldPosition.Distance( Target.WorldPosition );
		if ( Computer.IsValid() && Computer.Started )
		{
			Agent.MoveTo( Target.WorldPosition );

			if ( distance <= 70f )
				Target.Die();
		}

		if ( _nextSound )
		{
			Sound.Play( ScarySound, WorldPosition ).Volume = MathX.Remap( distance, 100f, 1000f, 2f, 0.1f );
			_nextSound = 1f;
		}
	}
}
