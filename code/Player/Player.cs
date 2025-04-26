namespace CryptidHunt;

public sealed class Player : Component
{
	[Property]
	public SkinnedModelRenderer Model { get; set; }

	[Property]
	public PlayerController Controller { get; set; }

	public bool LockInputs { get; set; } = true;
	public int HP { get; set; } = 3;

	protected override void OnStart()
	{
		if ( Model.IsValid() )
			Model.OnFootstepEvent += OnFootstepEvent;
	}

	TimeUntil _nextFootstep;
	TimeSince _lastBreath;
	bool _breatheIn;
	public TimeUntil NextInteraction { get; set; }

	protected override void OnFixedUpdate()
	{
		if ( !Controller.IsValid() ) return;

		if ( Controller.IsClimbing && _nextFootstep )
		{
			_nextFootstep = 0.3f;
			Sound.Play( "footstep-metal", WorldPosition ).Volume *= 7;
		}

		if ( Input.Pressed( "use" ) )
		{

			if ( NextInteraction )
			{
				NextInteraction = 0.5f;

				/*
				if ( InteractingWith != null )
				{

					InteractingWith.Interact( this );
					ShakeCamera();

				}
				else
				{

					var availableInteractable = FirstInteractable;

					availableInteractable?.Interact( this );
					ShakeCamera();

				}

				LastInteraction = 0;*/

			}

		}

		if ( Input.Pressed( "primary" ) )
		{
			if ( NextInteraction )
			{
				/*
				if ( Holding != null )
				{

					LastInteraction = 0;
					Holding.Use( this );

				}
				*/
			}

		}

		if ( HP > 0 && !LockInputs )
		{
			var breathTime = Math.Max( 1, Controller.Velocity.WithZ( 0f ).Length / 80 );

			if ( _lastBreath >= breathTime )
			{
				_breatheIn = !_breatheIn;

				Sound.Play( _breatheIn ? "breathe_in" : "breathe_out", WorldPosition );

			}
		}
	}

	public void Respawn()
	{

		//CurrentCheckpoint ??= PlayerSpawn.Initial;

		//EnableAllCollisions = true;
		//EnableDrawing = true;

		//WorldPosition = CurrentCheckpoint.Position;
		//Rotation = CurrentCheckpoint.Rotation;

	}


	bool _stepSound = false;

	public void OnFootstepEvent( SceneModel.FootstepEvent footstepEvent )
	{
		if ( !Controller.IsValid() ) return;

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

		Sound.Play( sound, footTrace.EndPosition ).Volume *= Controller.Velocity.WithZ( 0f ).Length / 10f;
	}
}
