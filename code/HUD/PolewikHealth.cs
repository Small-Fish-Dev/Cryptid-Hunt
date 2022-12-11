namespace SpookyJam2022;

public class PolewikHealth : WorldPanel
{
	public static PolewikHealth Instance { get; private set; }

	float lastHealth = 0f;
	Polewik polewik;
	TimeSince lastDamaged = 0f;

	Panel inner;

	public PolewikHealth( Polewik polewik )
	{
		this.polewik = polewik;
		StyleSheet = HUD.Instance.StyleSheet;

		var panelSize = new Vector2( 800, 100 );
		PanelBounds = new Rect( -panelSize / 2f, panelSize );

		inner = AddChild<Panel>( "inner" );
	}

	[Event.Client.Frame]
	private static void onframe()
	{
		if ( HUD.Instance == null ) return;

		if ( Instance == null )
		{
			var polewik = Entity.All.OfType<Polewik>().FirstOrDefault();
			if ( polewik != null ) 
				Instance = new PolewikHealth( polewik );
			else return;
		}
		
		if ( Instance.polewik == null || !Instance.polewik.IsValid )
		{
			Instance.Delete();
			Instance = null;
			return;
		}

		if ( Instance.lastHealth != Instance.polewik.hp )
			Instance.lastDamaged = 0f;

		Instance.Position = Instance.polewik.CollisionWorldSpaceCenter + Vector3.Up * 80f;
		Instance.Rotation = Rotation.LookAt( Camera.Current.Position - Instance.polewik.CollisionWorldSpaceCenter );

		Instance.lastHealth = Instance.polewik.hp;
		Instance.Style.Opacity = Instance.lastDamaged > 10f ? 0f : 1f;

		Instance.inner.Style.Width = Length.Fraction( Instance.polewik.hp / 100f );
	}
}
