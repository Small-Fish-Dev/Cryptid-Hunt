namespace SpookyJam2022;

class BlackScreen : Panel
{

	TimeSince startTime = 0f;
	float duration = 5.5f;

	public BlackScreen()
	{

		startTime = 0f;

	}

	[Event.Frame]
	void calculateOpacity()
	{

		if ( startTime >= duration )
		{

			Delete();

		}

	}

}
