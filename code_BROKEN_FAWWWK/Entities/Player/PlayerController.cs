﻿namespace SpookyJam2022;

public partial class PlayerController : PawnController
{
	[Net] public float Stamina { get; set; } = 100f;
	public TimeSince LastRan { get; private set; } = 0f;

	private Vector3 gravity => new Vector3( 0, 0, -700f );
	private float stepSize => 16f;

	TimeSince lastStep = 0f;

	private bool isTouchingLadder = false;
	private Vector3 ladderNormal = Vector3.Zero;

	private float getSpeed()
	{
		if ( Pawn is not Player pawn ) return 120f;

		if ( Input.Down( InputButton.Run ) && Stamina > 0.1f )
		{
			var overweight = MathF.Max( pawn.Inventory.Weight - pawn.Inventory.MaxWeight, 0f );
			return MathF.Max( 220f - overweight * 4f, 110f );
		}
		else return 120f;
	}

	private void CheckLadder()
	{
		/*
		var wishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
		wishVelocity = wishVelocity.Normal * Rotation.FromYaw( Input.Rotation.Yaw() );
		 */
		if ( isTouchingLadder )
		{
			if (Input.Pressed(InputButton.Jump))
			{
				Velocity = ladderNormal * 100.0f;
				isTouchingLadder = false;

				return;

			}
			else if (GroundEntity != null)
			{
				isTouchingLadder = false;

				return;
			}
		}

		var wishVelocity = EyeRotation.Angles().WithPitch( 0 ).Forward;
		
		var trace = Trace.Ray( Position, Position + wishVelocity )
			.Size( Player.CollisionBox.Mins, Player.CollisionBox.Maxs )
			.WithTag("ladder")
			.Ignore(Pawn)
			.Run();
		isTouchingLadder = trace.Hit;
		if (isTouchingLadder)
			ladderNormal = trace.Normal;
	}

	public override void Simulate()
	{
		if ( Pawn is not Player player ) return;
		EyeRotation = Rotation.From(player.InputLook);
		Rotation = Rotation.FromYaw( EyeRotation.Yaw() );

		CheckLadder();

		MoveHelper helper;
		if ( isTouchingLadder )
		{
			#region Ladder
			var wishVelocity = new Vector3( 0, player.InputDirection.y, player.InputDirection.x ); // W to ascend, S to descend
			wishVelocity = wishVelocity.Normal * Rotation.FromYaw( EyeRotation.Yaw() );
			var velocity = wishVelocity * 100;
			float normalDot = velocity.Dot( ladderNormal );
			var cross = ladderNormal * normalDot;
			Velocity = (velocity - cross) + (-normalDot * ladderNormal.Cross( Vector3.Up.Cross( ladderNormal ).Normal ));

			helper = new( Position, Velocity );
			helper.Trace = helper.Trace.Size( Player.CollisionBox.Mins, Player.CollisionBox.Maxs ).Ignore( Pawn );

			helper.TryMove( Time.Delta );

			Position = helper.Position;
			Velocity = helper.Velocity;
			#endregion
		}
		else
		{
			#region Movement
			var wishVelocity = new Vector3( player.InputDirection.x, player.InputDirection.y, 0 );
			wishVelocity = wishVelocity.Normal * Rotation.FromYaw( EyeRotation.Yaw() );
			wishVelocity *= getSpeed();
			Velocity = Vector3.Lerp( Velocity, wishVelocity, 12f * Time.Delta )
				.WithZ( Velocity.z );

			if ( GroundEntity == null )
				Velocity += gravity * Time.Delta;
			else
			{
				// jump ? i think
			}

			helper = new( Position, Velocity ) { MaxStandableAngle = 60f };
			helper.Trace = helper.Trace.Size( Player.CollisionBox.Mins, Player.CollisionBox.Maxs )
				.Ignore( Pawn );
			helper.TryUnstuck();
			helper.TryMoveWithStep( Time.Delta, stepSize );

			//DebugOverlay.ScreenText( $"{Velocity} {helper.Velocity}" );

			Position = helper.Position;
			Velocity = helper.Velocity;
			#endregion
		}


		if ( Velocity.z <= stepSize )
		{
			var tr = helper.TraceDirection( Vector3.Down * 2f );
			GroundEntity = tr.Entity;

			if ( tr.Entity != null )
			{
				Position += tr.Distance * Vector3.Down;

				if ( Velocity.z < 0f )
					Velocity = Velocity.WithZ( 0f );
			}
		}
		else GroundEntity = null;

		if ( Game.IsClient ) return;

		if ( Input.Down( InputButton.Run ) )
		{ 
			if ( Velocity.Length > 10f && Stamina > 0f )
			{
				Stamina = MathF.Max( Stamina - Time.Delta * 4f, 0f );
				LastRan = 0f;
			}
		}
		else if ( LastRan > 4f ) Stamina = MathF.Min( Stamina + Time.Delta * 5f, 100f );

		if ( Velocity.Length > 0f && lastStep >= 50 / Velocity.Length && GroundEntity != null )
		{

			var trace = Trace.Ray( Position, Position + Vector3.Down * 10f )
				.Ignore( Pawn )
				.Run();

			var surface = trace.Surface;
			var sound = surface.Sounds.FootLand;

			Sound.FromEntity( sound, Pawn )
				.SetVolume( (surface.ResourceName == "wood" ? 3 : 0.3f) * Velocity.Length / 80f );
			lastStep = 0f;
				
		}
	}

	public override void FrameSimulate()
	{
		
	}
}
