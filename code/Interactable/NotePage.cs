namespace CryptidHunt;

public partial class NotePage : Interactable
{
	[Property]
	[TextArea]
	public string Text { get; set; } = "Hello World";
	[Property]
	public bool BloodyPrint { get; set; } = false;

	public bool IsOpen = false;

	public override void Interact( Player player )
	{
		OnInteract.Invoke();

		if ( !IsOpen )
		{
			GameUI.OpenNote( Text, BloodyPrint );

			IsOpen = true;
			player.LockInputs = true;

			Sound.Play( "page_open", WorldPosition );
		}
		else
		{
			GameUI.CloseNote();

			IsOpen = false;
			player.LockInputs = false;

			/*

			var door = Entity.All.OfType<SceneTransferDoor>().Where( x => x.CheckpoindIDTarget == 1 ).FirstOrDefault(); // The first door haha, don't care
			door.Unlock();

			if ( Text.Contains( "window" ) ) //hehehehe
			{

				Event.Run( "BrainReveal" );

			}*/

			Sound.Play( "page_close", WorldPosition );
		}

	}
}
