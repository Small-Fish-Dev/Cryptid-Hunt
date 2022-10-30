namespace SpookyJam2022;

class InputHint : Panel
{
	private static InputHint current;

	TimeSince startTime = 0f;
	float duration = 5.5f;

	public InputHint( string Name, float duration = 5f )
	{
		current?.Delete();
		current = this;

		var hint = AddChild<Panel>( "hint" );
		hint.AddChild<Label>().SetText( Name );

		startTime = 0f;
	}

	[Event.Frame]
	void calculateLife()
	{
		if ( startTime >= duration )
			Delete();
	}
}
