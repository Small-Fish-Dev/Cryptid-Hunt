namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/beartrap.vmdl" )]
[Display( Name = "Bear Trap", GroupName = "Items", Description = "Bear trap to trap bears or cryptids" )]
[Item( "bear_trap" )]
public partial class BearTrap : BaseInteractable
{

	public override string ModelPath => "models/items/beartrap.vmdl";
	public override string UseDescription => "Take Bear Trap";
	public override Vector3 PromptOffset3D => new Vector3( 0f, 20f, 0f );
	public override Vector2 PromptOffset2D => new Vector2( -20f, 0f );
	public override Rotation OffsetRotation => Rotation.From( new Angles( 0f, -70f, 0f ) );
	public override Vector3 OffsetPosition => new Vector3( 5f, -5f, -5f );
	[Net] public bool Triggered { get; set; } = false;
	public override void Interact( Player player )
	{

		base.Interact( player );
		Transmit = TransmitType.Always;

	}
	public override void Use( Player player )
	{

		var trace = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 130f )
			.Ignore( player )
			.Run();


		if ( trace.Hit && trace.Normal.z >= 0.4f )
		{

			base.Use( player );

			Parent = null;
			EnableAllCollisions = true;
			Position = trace.HitPosition;
			Rotation = Rotation.LookAt( trace.Normal ).RotateAroundAxis( Vector3.Right, -90f );
			EnableDrawing = true;
			EnableShadowCasting = true;

			player.Holding = null;

			PlaySound( "sounds/items/beartrap_set.sound" );

		}

	}

	[Event.Tick.Server]
	void calcLogic()
	{

		foreach ( var polewik in FindInSphere( Position, 70f ).OfType<Polewik>() )
		{

			if ( !Triggered )
			{

				Triggered = true;
				polewik.HP -= 10;

				CurrentSequence.Name = "clamp";
				PlaySound( "sounds/items/beartrap_trigger.sound" );

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 1f );

					Delete();

				} );

			}

		}

	}

}
