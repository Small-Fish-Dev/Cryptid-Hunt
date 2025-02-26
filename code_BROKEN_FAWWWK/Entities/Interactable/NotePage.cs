﻿namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/items/page.vmdl" )]
[Display( Name = "Note Page", GroupName = "Items", Description = "Player can read this note by interacting with it" )]
public partial class NotePage : BaseInteractable
{

	public override string ModelPath => "models/items/page.vmdl";
	public override string UseDescription => "Read";
	public override Vector3 PromptOffset3D => new Vector3( 0f, 0f, 0f );
	public override Vector2 PromptOffset2D => new Vector2( 20f, 30f );
	[Net, Property, Description( "What's written in the note, you can use line breaks" )]
	public string Text { get; set; } = "Hello World";
	[Net, Property, Description( "Does it have a bloody handprint on it?" )]
	public bool BloodyPrint { get; set; } = false;

	public bool IsOpen = false;

	public override void Spawn()
	{

		base.Spawn();

		if ( Text != null )
		{

			Text = Text.Replace( @"\n", "\n" ); // Silly hammer new line

		}

		if ( Name.Contains( "Brain" ) )
		{

			EnableDrawing = false;
			EnableAllCollisions = false;

		}

	}

	public override void Interact( Player player )
	{

		if ( !IsOpen )
		{

			OpenPage( To.Single( player ) );

			IsOpen = true;
			player.LockInputs = true;
			player.InteractingWith = this;
			
		}
		else
		{

			ClosePage( To.Single( player ) );

			IsOpen = false;
			player.LockInputs = false;
			player.InteractingWith = null;

			var door = Entity.All.OfType<SceneTransferDoor>().Where( x => x.CheckpoindIDTarget == 1 ).FirstOrDefault(); // The first door haha, don't care
			door.Unlock();

			if ( Text.Contains( "window" ) ) //hehehehe
			{

				Event.Run( "BrainReveal" );

			}

		}

	}

	[ClientRpc]
	public void OpenPage()
	{

		Event.Run( "HideNotePage" );
		Event.Run( "CreateNotePage", Text, BloodyPrint );

		Sound.FromScreen( "sounds/items/page_open.sound" );

	}

	[ClientRpc]
	public void ClosePage()
	{

		Event.Run( "HideNotePage" );

		Sound.FromScreen( "sounds/items/page_close.sound" );

	}

}
