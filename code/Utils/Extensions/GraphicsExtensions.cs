namespace SpookyJam2022;

public static partial class Extensions
{
	/// <summary>
	/// Draws an outline box.
	/// </summary>
	/// <param name="rect"></param>
	/// <param name="col"></param>
	/// <param name="thickness"></param>
	/// <param name="attributes"></param>
	public static void DrawOutline( Rect rect, Color col, float thickness = 1, RenderAttributes attributes = null )
	{
		var rects = new Rect[]
		{
			new( rect.Left, rect.Top, rect.Width, thickness ),
			new( rect.Left, rect.Top + thickness, thickness, rect.Height - thickness * 2 ),
			new( rect.Left, rect.Bottom - thickness, rect.Width, thickness ),
			new( rect.Right - thickness, rect.Top + thickness, thickness, rect.Height - thickness * 2 )
		};

		for ( int i = 0; i < rects.Length; i++ )
			Graphics.DrawQuad( rects[i].Floor(), Material.UI.Basic, col, attributes );
	}
}
