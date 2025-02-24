namespace SpookyJam2022;

[HammerEntity]
[Display( Name = "Interactable Model", GroupName = "Cinematic", Description = "A trigger usable with Interact key, also displays a tip on the screen" )]
public partial class InteractableModel : BaseInteractable
{
	public override string UseDescription => _useDescription;
	[Property, Description( "Can this entity be interacted with?" )]
	public override bool Locked { get; set; } = false;
	public override string ModelPath => _modelPath;

	[Property, Description( "What to trigger when interacted with" )]
	public Output Trigger { get; set; }
	[Property, Description( "Can this entity be interacted with more than once" )]
	public bool SingleUse { get; set; } = false;

	[Property, Description( "What the text says when you look at the trigger" )]
	private string _useDescription { get; set; } = "Use";
	[Property, Description( "The model that represents this entity" )]
	private string _modelPath { get; set; } = "models/stuck_door.vmdl";

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
