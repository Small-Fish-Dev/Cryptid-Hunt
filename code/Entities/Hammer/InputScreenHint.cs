namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Input Hint", GroupName = "Cinematic", Description = "Entering this zone the first time will tell you how to play the game" )]
public partial class InputScreenHint : BaseTrigger
{

	[Net, Property, DefaultValue( "to not do anything" ), Description( "What the text says when you enter this area")]
	public string Hint { get; set; }
	[Net, Property, DefaultValue( "Jump" ), Description( "What to press" )]
	public string ButtonHint { get; set; }

	[Net] public bool Activated { get; set; } = false;

	public InputScreenHint() { }

	public override void StartTouch( Entity other )
	{

		base.Touch( other );

		if ( Host.IsClient ) return;
		if ( Activated ) return;
		if ( other is not Player player ) return;

		Activated = true;

		Game.Instance.StartInputHint( ButtonHint, Hint );

	}

}
