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


	protected override void OnFixedUpdate()
	{
		if ( Computer.IsValid() && Computer.Started )
		{
			Agent.MoveTo( Target.WorldPosition );

			if ( WorldPosition.Distance( Target.WorldPosition ) <= 100f )
				Target.Die();
		}
	}
}
