namespace CryptidHunt;

public partial class Player
{
    /*
	[Net] public BaseInteractable Holding { get; set; }
	public ModelEntity ViewModel { get; set; }
	public ModelEntity GhostEntity { get; set; }

	public void ChangeHolding( BaseInteractable entity )
	{

		if ( Holding != null )
		{

			Holding.EnableDrawing = false;
			Holding.EnableAllCollisions = false;
			Holding.Position = Position;
			Holding.Parent = this;
			Holding.EnableShadowCasting = false;

		}

		if ( entity != null )
		{

			Holding = entity;
			Holding.EnableDrawing = false;
			Holding.EnableAllCollisions = false;
			Holding.Position = Position;
			Holding.Parent = this;
			Holding.EnableShadowCasting = false;

		}

	}

	[Event( "PostCameraSetup" )]
	void computeHolding( Vector3 posDiff, Rotation rotDiff )
	{
		ViewModel ??= new ModelEntity();

		if ( Holding == null )
		{

			if ( ViewModel != null )
			{

				ViewModel.EnableDrawing = false;

			}

			if ( GhostEntity != null )
			{

				GhostEntity.EnableDrawing = false;

			}

			return;

		}
		else
		{

			if ( ViewModel != null )
			{

				ViewModel.EnableDrawing = true;

			}

			if ( Holding is BearTrap trap )
			{

				var trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 160f )
				.Ignore( this )
				.Run();


				if ( trace.Hit && trace.Normal.z >= 0.4f )
				{

					GhostEntity ??= new ModelEntity( trap.GetModelName() );

					if ( GhostEntity != null )
					{

						GhostEntity.SetMaterialOverride( "materials/dev/primary_white_emissive.vmat" );
						GhostEntity.RenderColor = Color.Gray.WithAlpha( 0.7f );

						GhostEntity.EnableDrawing = true;
						GhostEntity.Position = trace.HitPosition;
						GhostEntity.Rotation = Rotation.LookAt( trace.Normal ).RotateAroundAxis( Vector3.Right, -90f );

						GhostEntity.ResetInterpolation();

					}

				}
				else
				{

					if ( GhostEntity != null )
					{

						GhostEntity.EnableDrawing = false;

					}

				}
			}
			else
			{



				if ( GhostEntity != null )
				{

					GhostEntity.EnableDrawing = false;

				}

			}

		}
		
		if ( ViewModel.GetModelName() != Holding.GetModelName() )
		{

			ViewModel.SetModel( Holding.GetModelName() );

		}

		ViewModel.Rotation = EyeRotation - rotDiff;
		ViewModel.Position = EyePosition + ViewModel.Rotation.Forward * 30f + EyeRotation.Right * 10f + EyeRotation.Down * 10f;

		ViewModel.Rotation = ViewModel.Transform.RotationToWorld( Holding.OffsetRotation );
		ViewModel.Position = ViewModel.Transform.PointToWorld( Holding.OffsetPosition );
		

	}
	*/
}
