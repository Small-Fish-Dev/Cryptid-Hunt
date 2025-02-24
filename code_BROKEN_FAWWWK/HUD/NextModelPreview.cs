namespace SpookyJam2022;

class NextModelPreview : Panel
{

	public NextModelPreview()
	{

		AddEventListener( "onclick", () =>
		{

			Sound.FromScreen( "sounds/ui/button_click.sound" );

			Event.Run( "NextPreview" );

		} );

	}

}
