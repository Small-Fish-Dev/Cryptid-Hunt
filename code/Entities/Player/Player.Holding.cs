namespace SpookyJam2022;

public partial class Player
{

	[Net] public BaseInteractable Holding { get; set; }
	public ModelEntity ViewModel { get; set; }

	public void ChangeHolding( BaseInteractable entity )
	{

		if ( Holding != null )
		{

			Holding.EnableDrawing = false;
			Holding.EnableAllCollisions = false;
			Holding.Position = Position;
			Holding.Parent = this;

		}

		if ( entity != null )
		{

			Holding = entity;
			Holding.EnableDrawing = false;
			Holding.EnableAllCollisions = false;
			Holding.Position = Position;
			Holding.Parent = this;

		}

	}

	[Event( "PostCameraSetup" )]
	void computeHolding()
	{
		ViewModel ??= new ModelEntity();

		if ( Holding == null )
		{

			ViewModel.EnableDrawing = false;
			return;

		}
		else
		{

			ViewModel.EnableDrawing = true;

		}
		
		if ( ViewModel.GetModelName() != Holding.GetModelName() )
		{

			ViewModel.SetModel( Holding.GetModelName() );

		}

		ViewModel.Position = EyePosition + EyeRotation.Forward * 30f + EyeRotation.Right * 10f + EyeRotation.Down * 10f;
		ViewModel.Rotation = EyeRotation;

	}

}
