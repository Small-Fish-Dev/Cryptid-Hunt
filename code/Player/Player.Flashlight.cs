namespace CryptidHunt;

public partial class Player
{
	[Property]
	public Light FlashLight { get; set; }
	public bool FlashLightOn { get; set; } = true;

	public void SetFlashLight( bool on, bool sound = false )
	{
		if ( FlashLightOn != on )
		{
			FlashLightOn = on;

			if ( sound )
				Sound.Play( "button_click", Camera.WorldPosition );

			FlashLight.Enabled = FlashLightOn;
		}
	}
}
