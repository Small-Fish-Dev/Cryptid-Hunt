namespace CryptidHunt;

public partial class InputScreenHint : Component, Component.ITriggerListener
{

	[Property]
	public string Hint { get; set; } = " to not do anything";
	[Property]
	[InputAction]
	public string ButtonHint { get; set; } = "Use";

	public bool Activated { get; set; } = false;

	public InputScreenHint() { }


	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Parent.Components.TryGet<Player>( out var player, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		GameUI.OpenInputHint( Hint, ButtonHint );
	}

}
