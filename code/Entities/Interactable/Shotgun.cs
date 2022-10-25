namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/placeholders/placeholder_shotgun.vmdl" )]
[Display( Name = "Shotgun", GroupName = "Items", Description = "Shotgun shoot pew" )]
public partial class Shotgun : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_shotgun.vmdl";
	public override string UseDescription => "Take Shotgun";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );

	public override void Interact( Player player )
	{

		base.Interact( player );

	}

	public int BulletsPerShot => 20;
	public float Spread => 12f; // Degrees
	public float DamagePerBullet => 1f;
	public float DamageFalloff => 2000f; // How far until bullets deal no damage, Shotgun max range

	public override void Use( Player player )
	{

		for ( int i = 0; i < BulletsPerShot; i++ )
		{

			var randomDir = new Vector3( 0f, Rand.Float( -Spread, Spread ) * (float)Math.Cos( Rand.Float( 0f, 2f * (float)Math.PI ) ), Rand.Float( -Spread, Spread ) * (float)Math.Sin( Rand.Float( 0f, 2f * (float)Math.PI ) ) );
			var trace = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.RotateAroundAxis( Vector3.Up, randomDir.y ).RotateAroundAxis( Vector3.Right, randomDir.z ).Forward * DamageFalloff )
				.Ignore( player )
				.Run();

			var force = ( 1f - trace.StartPosition.Distance( trace.EndPosition ) / DamageFalloff );

			DebugOverlay.Sphere( trace.EndPosition, 5f, new Color( force, 0, 0 ), 2f );
			
			if ( trace.Entity is Polewik polewik )
			{

				polewik.HP -= DamagePerBullet * force;
				Log.Info( polewik.HP );

			}


		}

	}

}
