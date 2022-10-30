using Sandbox;
using SpookyJam2022.States;

namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/brain/brain.vmdl" )]
[Display( Name = "Brain", GroupName = "Monster", Description = "Brain activates post game" )]
public partial class Brain : AnimatedEntity
{

	[Net] public bool Looking { get; set; } = false;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/brain/brain.vmdl" );
		EnableDrawing = false;
	}

	[Event( "StartCutscene" )]
	public void StartCutscene()
	{

		EnableDrawing = true;

	}

	[Event( "RemovedCardboard")]
	public void RemovedCardboard()
	{

		GameTask.RunInThreadAsync( async () =>
		{

			await GameTask.DelaySeconds( 1.5f );

			Looking = true;

			await GameTask.DelaySeconds( 3f );

			Game.Instance.StartBlackScreen();

			await GameTask.DelaySeconds( 2.5f );

			Game.State = new CreditsMenuState();

		} );

	}


	[Event.Tick]
	public void watchPlayer()
	{

		var ply = Entity.All.OfType<Player>().OrderBy( x => x.Position.Distance( Position ) ).FirstOrDefault();
		
		if ( ply != null )
		{

			if ( Looking )
			{

				SetAnimParameter( "look", true );

			}

		}
	}

}
