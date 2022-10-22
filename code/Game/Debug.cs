namespace SpookyJam2022;

public partial class Game
{

	[ConCmd.Server]
	public static void SpawnNote()
	{

		if ( ConsoleSystem.Caller.Pawn is not Player pawn ) return;

		var tr = Trace.Ray( pawn.EyePosition, pawn.EyeRotation.Forward * 500f )
			.WorldAndEntities()
			.Ignore( pawn )
			.Run();

		var ent = new NotePage()
		{

			Position = tr.EndPosition,

		};

	}

}
