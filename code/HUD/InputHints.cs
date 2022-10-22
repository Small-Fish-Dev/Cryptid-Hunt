namespace Hamsteria;

/*public class InputHints : Panel
{
	private class InputHint : Panel
	{
		public Label Title;
		private string description;

		public InputHint( InputButton button )
		{

			Title = AddChild<Label>( "title" );

			var icon = AddChild<Panel>( "icon" );
			icon.Style.BackgroundImage = Input.GetGlyph( button, InputGlyphSize.Medium, GlyphStyle.Light.WithSolidABXY().WithNeutralColorABXY() );
		}
	}

	private Dictionary<InputButton, InputHint> hints = new();
	private List<InputButton> activeButtons = new();
	public InputHints()
	{
		foreach ( InputButton input in Enum.GetValues( typeof( InputButton ) ) )
		{

			var hint = new InputHint( input );
			hint.SetClass( "hidden", true );
			AddChild( hint );
			hints.Add( input, hint );

		}

	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player pawn ) return;

		foreach ( var interaction in pawn.AvailableInteractions )
		{

			hints[interaction.Key].SetClass( "hidden", false );
			hints[interaction.Key].Title.Text = interaction.Value.Interaction.Hint;
			activeButtons.Add( interaction.Key );

		}

		foreach( var hint in hints )
		{

			if ( !activeButtons.Contains( hint.Key )  )
			{

				hints[hint.Key].SetClass( "hidden", true );

			}

		}

		activeButtons.Clear();

	}

}
*/
