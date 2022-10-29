namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Sound Cue", GroupName = "Cinematic", Description = "Play sounds when player gets in here (Only plays once!)" )]
public partial class MySoundCue : BaseTrigger
{

	[Net, Property, ResourceType("sound")]
	public string SoundPath { get; set; }
	[Net, Property, DefaultValue( false ), Description( "If false, sounds will play the the pivot of this entity" )]
	public bool UI { get; set; }

	[Net] public bool Activated { get; set; } = false;

	public MySoundCue() { }

	public override void StartTouch( Entity other )
	{

		base.Touch( other );

		if ( Host.IsClient ) return;
		if ( Activated ) return;
		if ( other is not Player player ) return;

		Activated = true;

		if ( UI )
		{
			Sound.FromScreen( SoundPath );

		}
		else
		{

			Sound.FromWorld( SoundPath, Position );

		}

	}

}
