namespace SpookyJam2022;

class ZoneHint : Panel
{
	private static ZoneHint current;

	TimeSince startTime = 0f;
	float duration = 5.5f;

	public ZoneHint( string Name, float duration = 5f )
	{
		current?.Delete();
		current = this;

		var hint = AddChild<Panel>( "hint" );
		hint.AddChild<Label>().SetText( Name );

		startTime = 0f;
	}

	[Event.Client.Frame]
	void calculateLife()
	{
		if ( startTime >= duration )
			Delete();
	}
}
