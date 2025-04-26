namespace CryptidHunt;

public partial class Player : Component
{
    [Property]
    public SkinnedModelRenderer Model { get; set; }

    [Property]
    public PlayerController Controller { get; set; }

    public float RunSpeed { get; set; }
    public float Stamina { get; set; } = 100f;
    public bool Running { get; set; } = false;
    public TimeSince LastRan { get; set; } = 0f;
    public bool LockInputs { get; set; } = false;
    public int HP { get; set; } = 3;

    protected override void OnStart()
    {
        Log.Info("HELLO");
        if (Model.IsValid())
            Model.OnFootstepEvent += OnFootstepEvent;

        if (Controller.IsValid())
        {
            RunSpeed = Controller.RunSpeed;
            Controller.RunSpeed = Controller.WalkSpeed;
        }
    }

    TimeUntil _nextFootstep;
    TimeSince _lastBreath;
    bool _breatheIn;
    public TimeUntil NextInteraction { get; set; }

    protected override void OnFixedUpdate()
    {
        if (!Controller.IsValid()) return;

        if (Controller.IsClimbing && _nextFootstep)
        {
            _nextFootstep = 0.3f;
            Sound.Play("footstep-metal", WorldPosition).Volume *= 7;
        }

        if (Input.Down("Run") && Stamina > 0 && (Running || LastRan >= 1f))
            Running = true;
        else
            Running = false;

        if (Running)
        {
            Stamina = Math.Clamp(Stamina - Time.Delta * 10f, 0f, 100f);
            Controller.RunSpeed = RunSpeed;
            LastRan = 0f;
        }
        else
        {
            Controller.RunSpeed = Controller.WalkSpeed;
            Stamina = Math.Clamp(Stamina + Time.Delta * 10f, 0f, 100f);
        }

        if (Input.Pressed("use"))
        {

            if (NextInteraction)
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

        if (Input.Pressed("attack1"))
        {
            if (NextInteraction)
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

        if (HP > 0 && !LockInputs)
        {
            var breathTime = Running ? 0.7f : 1.5f;

            if (_lastBreath >= breathTime)
            {
                _breatheIn = !_breatheIn;
                _lastBreath = 0f;

                var sound = Sound.Play(_breatheIn ? "breathe_in" : "breathe_out", WorldPosition);
                sound.Volume *= Running ? 40f : 10f;
                sound.Pitch *= Running ? 1.1f : 1f;

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
    TimeSince _lastStep = 0f;

    public void OnFootstepEvent(SceneModel.FootstepEvent footstepEvent)
    {
        if (!Controller.IsValid()) return;

        var footTrace = Scene.Trace.Ray(WorldPosition, WorldPosition + Vector3.Down * 10f)
            .Radius(2f)
            .IgnoreDynamic()
            .IgnoreGameObjectHierarchy(GameObject)
            .Run();

        if (!footTrace.Hit) return;

        var tag = footTrace.Tags
            .Where(x => x != "solid" && x != "world")
            .FirstOrDefault();

        _stepSound = !_stepSound;

        if (!_stepSound)
            return;

        if (_lastStep <= 0.2f) return;
        _lastStep = 0f;

        var sound = tag switch
        {
            "metal" => "footstep-metal",
            "grass" => "footstep-grass",
            "dirt" => "footstep-dirt",
            _ => "footstep-concrete"
        };

        Sound.Play(sound, footTrace.EndPosition).Volume *= Controller.Velocity.WithZ(0f).Length / 10f;
    }
}
