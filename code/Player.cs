using Sandbox;

public sealed class Player : Component
{
	[Property]
	public SkinnedModelRenderer Model { get; set; }

	[Property]
	public PlayerController Controller { get; set; }

	protected override void OnStart()
	{
		if ( Model.IsValid() )
			Model.OnFootstepEvent += OnFootstepEvent;
	}

	protected override void OnUpdate()
	{

	}

	bool _stepSound = false;

	public void OnFootstepEvent( SceneModel.FootstepEvent footstepEvent )
	{
		var footTrace = Scene.Trace.Ray( WorldPosition, WorldPosition + Vector3.Down * 10f )
			.Radius( 2f )
			.IgnoreDynamic()
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( !footTrace.Hit ) return;

		var tag = footTrace.Tags
			.Where( x => x != "solid" && x != "world" )
			.FirstOrDefault();

		_stepSound = !_stepSound;

		if ( !_stepSound )
			return;

		var sound = tag switch
		{
			"metal" => "footstep-metal",
			"grass" => "footstep-grass",
			"dirt" => "footstep-dirt",
			_ => "footstep-concrete"
		};

		Sound.Play( sound, footTrace.EndPosition ).Volume *= Controller.Velocity.Length / 10f;
	}
}
