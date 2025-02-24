namespace SpookyJam2022;

public partial class BaseInteractable : AnimatedEntity
{

	public virtual string ModelPath => "models/placeholders/placeholder_cinder.vmdl";
	public virtual string UseDescription => "Take";
	public virtual bool Locked { get; set; } = false;

	public float Amount { get; set; } = 1;
	public Item ActiveItem { get; set; }

	public virtual Vector3 PromptOffset3D => new Vector3( 0f );
	public virtual Vector2 PromptOffset2D => new Vector2( 0f );
	public virtual Vector3 OffsetPosition => new Vector3( 0f );
	public virtual Rotation OffsetRotation => new Rotation();


	public override void Spawn()
	{

		SetModel( ModelPath );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

	}

	public virtual void Interact( Player player )
	{
		var resource = Item.GetByType( GetType() );
		if ( resource != null )
		{
			player.Inventory?.Insert( Item.FromResource( resource.ResourceName ), Amount );
			Sound.FromScreen( "sounds/items/pickup.sound" );
			Delete();

		}
		else
		{
			return;
		}
	}

	public virtual void Use( Player player )
	{
		if ( player == null
			|| ActiveItem == null
			|| ActiveItem.Container != player.Inventory ) return;

		var index = player.Inventory.Items.IndexOf( ActiveItem );
		ShakeCamera( player );
		player.Inventory.Remove( index );
	}

	[ClientRpc]
	public void ShakeCamera( Player player )
	{

		Event.Run( "ScreenShake", 0.15f, 4f );

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


	public void Lock()
	{

		Locked = true;
		_lock();

	}

	[ClientRpc]
	void _lock()
	{

		Locked = true;

	}


}
