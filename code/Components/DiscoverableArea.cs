using System;
using static System.Net.Mime.MediaTypeNames;

namespace CryptidHunt;

public partial class DiscoverableArea : Component, Component.ITriggerListener
{
	[Property]
	public string AreaName { get; set; } = "World";
	public bool Activated { get; set; } = false;

	public DiscoverableArea() { }

	public void OnTriggerEnter( Collider other )
	{
		if ( !Active || Activated ) return;
		if ( !other.GameObject.Parent.Components.TryGet<Player>( out var player, FindMode.EnabledInSelf ) ) return;

		Activated = true;

		GameUI.OpenZoneHint( AreaName );
	}
}
