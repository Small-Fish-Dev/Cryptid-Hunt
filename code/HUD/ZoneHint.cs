namespace SpookyJam2022;

class ZoneHint : Panel
{

	TimeSince startTime = 0f;
	float duration = 5.5f;

	public ZoneHint( string Name, float duration = 5f )
	{

		AddChild<Label>( "hint" ).SetText( Name );

		startTime = 0f;

	}

	[Event.Frame]
	void calculateLife()
	{

		if ( startTime >= duration )
		{

			Delete();

		}

	}

}
