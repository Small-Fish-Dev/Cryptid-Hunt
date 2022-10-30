namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Interactable Model", GroupName = "Cinematic", Description = "A trigger usable with Interact key, also displays a tip on the screen" )]
public partial class InteractableModel : BaseInteractable
{
	public override string UseDescription => _useDescription;
	[Property, DefaultValue(false), Description("Can this entity be interacted with?")]
	public override bool Locked { get; set; }
	public override string ModelPath => _modelPath;
	
	[Property, DefaultValue( "" ), Description( "What to trigger when interacted with" )]
	public Output Trigger { get; set; }
	[Property, DefaultValue( false ), Description( "Can this entity be interacted with more than once" )]
	public bool SingleUse { get; set; }

	[Property, DefaultValue( "Use" ), Description( "What the text says when you look at the trigger" )]
	private string _useDescription { get; set; }
	[Property, DefaultValue( "models/stuck_door.vmdl" ), Description( "The model that represents this entity" )]
	private string _modelPath { get; set; }
	
	public override void Interact( Player player )
	{

		if ( !Locked )
		{

			Trigger.Fire( this, 0 );
			
			if (SingleUse)
				Delete();

		}

	}
}
