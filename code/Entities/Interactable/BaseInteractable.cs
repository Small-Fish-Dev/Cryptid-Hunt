namespace SpookyJam2022;

public partial class BaseInteractable : ModelEntity
{

	public virtual string ModelPath => "models/placeholders/placeholder_cinder.vmdl";
	public virtual string UseDescription => "Take";
	public virtual bool Locked { get; set; } = false;

	public virtual Vector3 PromptOffset3D => new Vector3( 0f );
	public virtual Vector2 PromptOffset2D => new Vector2( 0f );

	public override void Spawn()
	{

		SetModel( ModelPath );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

	}

	public virtual void Interact( Player player )
	{

		player.ChangeHolding( this );

	}

	public void Unlock()
	{

		Locked = false;
		unlock();

	}

	[ClientRpc]
	void unlock()
	{

		Locked = false;

	}


}
