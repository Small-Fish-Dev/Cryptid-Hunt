namespace SpookyJam2022;

public partial class InteractableNotePage : BaseInteractable
{

	public override string ModelPath => "models/placeholders/placeholder_page.vmdl";
	public override string UseDescription => "Read";
	public override Vector3 PromptOffset3D => new Vector3( 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 0f );
	public string Text => "Help!!!\nI am going to die here\nTHE MONSTER IS COMING\nIt comes from that damn Ape Tavern\nOH NO THEY FOUND ME!\n\n\nHEEELP\nOH NOOOO\nAAACK\n\n\nugh\noof\nack\nWAAAAA!!!";
	public bool BloodyPrint => true;

	public bool IsOpen = false;

	public override void Interact( Player player )
	{

		if ( !IsOpen )
		{

			OpenPage( To.Single( player ) );

			IsOpen = true;
			player.LockInputs = true;
			
		}
		else
		{

			ClosePage( To.Single( player ) );

			IsOpen = false;
			player.LockInputs = false;

		}

	}

	[ClientRpc]
	public void OpenPage()
	{

		Event.Run( "HideNotePage" );
		Event.Run( "CreateNotePage", Text, BloodyPrint );

	}

	[ClientRpc]
	public void ClosePage()
	{

		Event.Run( "HideNotePage" );

	}

}
