using Sandbox;

namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/shotgun.vmdl" )]
[Display( Name = "Shotgun", GroupName = "Items", Description = "Shotgun shoot pew" )]
[Item( "shotgun" )]
public partial class Shotgun : BaseInteractable
{

	public override string ModelPath => "models/items/shotgun.vmdl";
	public override string UseDescription => "Take Shotgun";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );
	public override Rotation OffsetRotation => Rotation.From( new Angles( 0f, 90f, 0f ) );
	public override Vector3 OffsetPosition => new Vector3( 0f, 0f, -10f );

	public override void Interact( Player player )
	{

		base.Interact( player );

	}

	public int BulletsPerShot => 22;
	public float Spread => 8f; // Degrees
	public float DamagePerBullet => 1f;
	public float DamageFalloff => 2000f; // How far until bullets deal no damage, Shotgun max range

	public override void Use( Player player )
	{

		var totalDamage = 0f;
		Polewik victim = null;

		var ammoIndex = player.Inventory?.Find( "shotgun_ammo" );
		if ( ammoIndex == null ) return;

		var success = player.Inventory?.Remove( ammoIndex.Value, 1 ) ?? false;
		if ( !success ) return;

		PlaySound( "weapons/rust_pumpshotgun/sounds/rust_pumpshotgun.shoot.sound" ).SetVolume( 3 );
		CreateParticle( player );

		for ( int i = 0; i < BulletsPerShot; i++ )
		{

			var randomDir = new Vector3( 0f, Rand.Float( -Spread, Spread ) * (float)Math.Cos( Rand.Float( 0f, 2f * (float)Math.PI ) ), Rand.Float( -Spread, Spread ) * (float)Math.Sin( Rand.Float( 0f, 2f * (float)Math.PI ) ) );
			var trace = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.RotateAroundAxis( Vector3.Up, randomDir.y ).RotateAroundAxis( Vector3.Right, randomDir.z ).Forward * DamageFalloff )
				.Ignore( player )
				.Run();

			trace.Surface.DoBulletImpact( trace );

			if ( trace.Entity is Polewik polewik )
			{

				totalDamage += DamagePerBullet;

				victim = polewik;

				CreateBlood( trace.EndPosition );

			}


		}

		GameTask.RunInThreadAsync( async () =>
		{

			await Task.DelaySeconds( 0.6f );
			PlaySound( "weapons/rust_pumpshotgun/sounds/rust_pumpshotgun.pump.sound" ).SetVolume( 3 );

		} );

		player.LastInteraction = -1f;

		if ( victim != null )
		{

			victim.HP -= totalDamage;

		}


	}

	[ClientRpc]
	public void CreateParticle( Player player )
	{

		Particles.Create( "particles/pistol_muzzleflash.vpcf", player.ViewModel, "muzzle" );
		Event.Run( "ScreenShake", 0.2f, 15f );


		GameTask.RunInThreadAsync( async () =>
		{

			await Task.DelaySeconds( 0.6f );

			Event.Run( "ScreenShake", 0.3f, 5f );

		} );

	}


	[ClientRpc]
	public void CreateBlood( Vector3 position )
	{

		Particles.Create( "particles/bigblood.vpcf", position );

	}

}
