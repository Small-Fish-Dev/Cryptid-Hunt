using Sandbox.Internal;

namespace SpookyJam2022;

[HammerEntity]
[EditorModel( "models/editor/camera.vmdl" )]
[Display( Name = "Scripted Event Camera", GroupName = "Cinematic", Description = "Set this up where you want the camera to be during scripted events" )]
public partial class ScriptedEventCamera : Entity
{

	[Net] public Polewik Target { get; set; }
	[Net] public bool DoNotLerp { get; set; } = false;

	public ScriptedEventCamera() { }

	public ScriptedEventCamera( Polewik target, bool doNotLerp = false )
	{

		Target = target;
		DoNotLerp = doNotLerp;

	}

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	SpotLightEntity light { get; set; }

	[Event.Tick.Server]
	void calcTarget()
	{

		if ( Target != null )
		{

			Position = Target.Transform.PointToWorld( new Vector3( 70f, 0f, 72f ) );
			Rotation = Rotation.LookAt( Target.GetBoneTransform( Target.GetBoneIndex( "camera" ) ).Position - Position );

		}

		if ( light == null && Name.Contains( "Lake" ) )
		{
			light = new SpotLightEntity
			{
				Enabled = false,
				DynamicShadows = true,
				Range = 2000,
				Brightness = 2f,
				Color = Color.White,
				InnerConeAngle = 20,
				OuterConeAngle = 30,
				FogStrength = 0.0f,
				Owner = Owner,
				LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" )

			};

			light.SetLightEnabled( true );
			light.Transmit = TransmitType.Always;
			light.Position = Position;
			light.Rotation = Rotation;

			light.Parent = this;

			foreach ( var polewik in Entity.All.OfType<Polewik>())
			{

				polewik.SetAnimParameter( "howl", true );

				GameTask.RunInThreadAsync( async () =>
				{

					await GameTask.DelaySeconds( 0.1f );

					polewik.SetAnimParameter( "howl", false );

				} );

			}

		}

	}

	[Event( "ScriptedEventStart" )]
	void addLight()
	{

		if ( light != null && Name.Contains( "Lake" ) )
		{

			light.Enabled = true;

		}

	}

	[Event("ScriptedEventEnd")]
	void removeLight()
	{

		if ( light != null && Name.Contains( "Lake" ) )
		{

			light.Delete(); 
			
			foreach ( var polewik in Entity.All.OfType<Polewik>() )
			{

				polewik.Disabled = false;
				polewik.CurrentState = PolewikState.AttackPersistent;

				return;

			}

		}

	}

}
