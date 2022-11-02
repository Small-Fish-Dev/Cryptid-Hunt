using SpookyJam2022.States;
using SpookyJam2022.Utils;

namespace SpookyJam2022;

public partial class NextBot : ModelEntity
{
	public float Speed => 400f;
	public NavAgentHull Agent => NavAgentHull.Agent1;

	public NextBotPlayer Target => All.OfType<NextBotPlayer>().FirstOrDefault();
	private WorldPanel panel;

	private TimeSince lastKilled = 0f;

	public Sound Music;
	
	public override void Spawn()
	{
		Position = All.OfType<NextbotSpawn>().FirstOrDefault()?.Position ?? Vector3.Zero;
		Transmit = TransmitType.Always;
		Music = Sound.FromScreen( "sounds/music/dayofchaos.sound" )
			.SetVolume( 0.2f );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		NextBotState.Nextbot = this;
	}

	protected override void OnDestroy()
	{
		Music.Stop();
		base.OnDestroy();
	}

	[Event.Tick]
	public void ComputeAI()
	{
		if ( IsClient )
		{
			if ( panel == null ) return;

			var eyeRot = Local.Pawn.EyeRotation;
			panel.Position = Position + Vector3.Up * 50f;
			panel.Rotation = eyeRot.RotateAroundAxis( Vector3.Up, 180 ); //Rotation.FromYaw( Rotation.LookAt( Local.Pawn.Position - Position ).Yaw() );

			return;
		}

		var maxDistanceSound = 1500f;
		var distance = Target.Position.Distance( Position );

		if ( Time.Tick % 30 == 0 && Target != null )
		{
			var volume = MathF.Max( (maxDistanceSound - distance) / maxDistanceSound, 0 );
			Sound.FromScreen( "sounds/scary/creepy_sound.sound" )
				.SetVolume( volume )
				.SetPitch( Rand.Float( 0.8f, 1.2f ) );
		}

		if ( Target == null && Target.LifeState == LifeState.Alive ) return;

		ComputeMovement();

		if ( distance <= 100f && lastKilled > 5f )
		{
			Target.TakeDamage( DamageInfo.Generic( 999f ) );
			lastKilled = 0f;

			Sound.FromScreen( "sounds/scary/creepy_sound.sound" )
				.SetVolume( 3f )
				.SetPitch( Rand.Float( 0.8f, 1.2f ) );
		}

	}

	public Vector3[] PathPoints;
	public int PathIndex;
	public Vector3 NextPosition;
	TimeSince lastCalculatedPath;

	public void ComputeMovement()
	{
		ComputePath();

		if ( lastCalculatedPath > 0.2f )
			NavigateTo( Target.Position );
	}

	public bool NavigateTo( Vector3 pos )
	{

		var path = NavMesh.PathBuilder( Position )
			.WithAgentHull( Agent )
			.Build( pos );

		if ( path == null || path.Segments.Count == 0 ) return false;

		PathIndex = 0;
		PathPoints = path.Segments.Select( segment => segment.Position ).ToArray();

		lastCalculatedPath = 0f;

		return true;

	}

	public virtual void ComputePath()
	{

		if ( PathPoints == null )
		{
			if ( Target != null )
			{
				var normal = (Target.Position - Position).Normal.WithZ( 0f );
				Position += normal * Speed * Time.Delta;
			}

			return;
		}

		NextPosition = PathPoints[Math.Clamp( PathIndex + 1, 0, PathPoints.Length - 1 )];

		if ( Position.Distance( NextPosition ) <= Speed * Time.Delta )
		{

			PathIndex++;

			if ( PathIndex == PathPoints.Length )
			{

				PathPoints = null;
				return;

			}

		}

		var dir = (NextPosition - Position).Normal.WithZ( 0f );
		Position += dir * Speed * Time.Delta;

	}

	[ClientRpc]
	public void SetImage( string prompt )
	{

		new Action( async () =>
		{
			var panelSize = new Vector2( 1600, 1600 );
			var texture = await Flickr.Get( prompt );
			panel = new();
			panel.WorldScale = 1f;
			panel.PanelBounds = new Rect( -panelSize / 2f, panelSize );

			var image = panel.AddChild<Panel>();
			image.Style.Width = Length.Percent( 100 );
			image.Style.Height = Length.Percent( 100 );
			image.Style.BackgroundSizeX = Length.Percent( 100 );
			image.Style.BackgroundSizeY = Length.Percent( 100 );
			image.Style.ImageRendering = ImageRendering.Point;
			image.Style.BackgroundImage = texture;
		} ).Invoke();

	}

}
