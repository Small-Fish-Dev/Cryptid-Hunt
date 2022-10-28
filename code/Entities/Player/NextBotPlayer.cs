using SpookyJam2022.States;

namespace SpookyJam2022;

public partial class NextBotPlayer : BasePlayer
{
	private readonly ClothingContainer _clothing = new();
	private readonly IEnumerable<NextbotPlayerSpawn> _spawnPoints;
	
	private DamageInfo _lastDamage;

	public NextBotPlayer()
	{
		_spawnPoints = Entity.All.OfType<NextbotPlayerSpawn>();
	}

	public override void Spawn()
	{
		base.Spawn();
		
		Controller ??= new WalkController();
		
		SetModel( "models/citizen/citizen.vmdl" );
		
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		
		_clothing.LoadFromClient( Game.PlayerClient );
		_clothing.DressEntity( this );
	}

	public override void Respawn()
	{
		Health = float.Epsilon; // Smallest health possible
		
		var randomSpawnPoint = _spawnPoints.MinBy(x => Guid.NewGuid());
		Position = randomSpawnPoint.Position;
		Rotation = randomSpawnPoint.Rotation;
		
		Event.Run( "nextbot.player.respawn" );
	}
	
	public override void TakeDamage( DamageInfo info )
	{
		_lastDamage = info;

		base.TakeDamage( info );
	}

	public override void OnKilled()
	{
		base.OnKilled();

		NextBotState.Deaths++;
		
		BecomeRagdollOnClient( Velocity, _lastDamage.Flags, _lastDamage.Position, _lastDamage.Force,
			_lastDamage.BoneIndex );

		Event.Run( "nextbot.player.dead" );
	}
	
	[ClientRpc]
	private void BecomeRagdollOnClient( Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone )
	{
		var ent = new ModelEntity();
		ent.Tags.Add( "ragdoll", "solid", "debris" );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Scale = Scale;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.CopyBodyGroups( this );
		ent.CopyMaterialGroup( this );
		ent.CopyMaterialOverrides( this );
		ent.TakeDecalsFrom( this );
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ent.RenderColor = RenderColor;
		ent.PhysicsGroup.Velocity = velocity;
		ent.PhysicsEnabled = true;

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			var model = e.GetModelName();

			var clothing = new ModelEntity();
			clothing.SetModel( model );
			clothing.SetParent( ent, true );
			clothing.RenderColor = e.RenderColor;
			clothing.CopyBodyGroups( e );
			clothing.CopyMaterialGroup( e );
		}

		if ( damageFlags.HasFlag( DamageFlags.Bullet ) ||
		     damageFlags.HasFlag( DamageFlags.PhysicsImpact ) )
		{
			PhysicsBody body = bone > 0 ? ent.GetBonePhysicsBody( bone ) : null;

			if ( body != null )
			{
				body.ApplyImpulseAt( forcePos, force * body.Mass );
			}
			else
			{
				ent.PhysicsGroup.ApplyImpulse( force );
			}
		}

		if ( damageFlags.HasFlag( DamageFlags.Blast ) )
		{
			if ( ent.PhysicsGroup != null )
			{
				ent.PhysicsGroup.AddVelocity( (Position - (forcePos + Vector3.Down * 100.0f)).Normal * (force.Length * 0.2f) );
				var angularDir = (Rotation.FromYaw( 90 ) * force.WithZ( 0 ).Normal).Normal;
				ent.PhysicsGroup.AddAngularVelocity( angularDir * (force.Length * 0.02f) );
			}
		}

		ent.DeleteAsync( 10.0f );
	}
}
