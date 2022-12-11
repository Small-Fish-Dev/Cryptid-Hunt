namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Discoverable Area", GroupName = "Cinematic", Description = "Entering this zone the first time will tell you the name on your screen" )]
public partial class DiscoverableArea : BaseTrigger
{

	[Net, Property, Description( "What the text says when you enter this area" )]
	public string AreaName { get; set; } = "World";

	[Net] public bool Activated { get; set; } = false;

	public DiscoverableArea() { }

	public override void StartTouch( Entity other )
	{

		base.Touch( other );

		if ( Game.IsClient ) return;
		if ( Activated ) return;
		if ( other is not Player player ) return;

		Activated = true;

		CryptidHunt.Instance.StartZoneHint( AreaName );

	}

}
