using Sandbox;

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

				var local = Transform.PointToLocal( ply.Position );
				SetAnimParameter( "lookat", local.WithX( Math.Max( local.x, 0 ) ) );

			}


		}
	}

}
