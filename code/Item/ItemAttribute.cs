namespace CryptidHunt;

[AttributeUsage( AttributeTargets.Class )]
public class ItemAttribute : Attribute
{
	public string Resource { get; private set; }

	public ItemAttribute( string resource )
	{
		Resource = resource;
	}
}
