namespace SpookyJam2022;

public partial class CryptidHunt
{

	[ConCmd.Server]
	public static void SpawnNote()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;

		var tr = Trace.Ray( pawn.EyePosition, pawn.EyePosition + pawn.EyeRotation.Forward * 500f )
			.WorldAndEntities()
			.Ignore( pawn )
			.Run();

		var ent = new NotePage()
		{

			Position = tr.EndPosition,

		};

	}

	[ConCmd.Server]
	public static void GiveItem( string resourceName )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;
		if ( pawn.Inventory == null ) return;

		pawn.Inventory.Insert( Item.FromResource( resourceName ) );
		//Log.Error( pawn.Inventory.ToString() );
	}

	[ConCmd.Server]
	public static void RemoveItem( int index, float? amount = null )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;
		if ( pawn.Inventory == null ) return;

		pawn.Inventory.Remove( index, amount );
		//Log.Error( pawn.Inventory.ToString() );
	}

	[ConCmd.Server]
	public static void Polewik()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;

		var tr = Trace.Ray( pawn.EyePosition, pawn.EyePosition + pawn.EyeRotation.Forward * 500f )
			.WorldAndEntities()
			.Ignore( pawn )
			.Run();

		var ent = new Polewik()
		{

			Position = tr.EndPosition,

		};

	}

	[ConCmd.Server]
	public static void Navigate()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;

		var tr = Trace.Ray( pawn.EyePosition, pawn.EyePosition + pawn.EyeRotation.Forward * 50000f )
			.WorldAndEntities()
			.Ignore( pawn )
			.Run();

		//DebugOverlay.Sphere( tr.EndPosition, 20f, Color.Blue );

		foreach( var polewik in Entity.All.OfType<Polewik>() )
		{

			polewik.NavigateTo( tr.EndPosition );

		}

	}

	[ConCmd.Server]
	public static void Patrol()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;

		var tr = Trace.Ray( pawn.EyePosition, pawn.EyePosition + pawn.EyeRotation.Forward * 50000f )
			.WorldAndEntities()
			.Ignore( pawn )
			.Run();

		foreach ( var polewik in Entity.All.OfType<Polewik>() )
		{

			polewik.CurrentState = PolewikState.Patrolling;

		}

	}

}
