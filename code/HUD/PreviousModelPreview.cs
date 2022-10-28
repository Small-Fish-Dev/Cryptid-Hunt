namespace SpookyJam2022;

class PreviousModelPreview : Panel
{

	public PreviousModelPreview()
	{

		AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Event.Run( "PreviousPreview" );

		} );

	}

}
