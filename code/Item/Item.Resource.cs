namespace SpookyJam2022;

public partial class Item
{
	public enum AmountType
	{
		Float = 0,
		Integer = 1
	}

	[GameResource( "Item", "item", "An item resource." )]
	public class ItemResource : GameResource
	{
		private static Dictionary<string, ItemResource> all = new();
		public static IReadOnlyDictionary<string, ItemResource> All => all;

		[Category( "Visual" )] public string Title { get; set; }
		[Category( "Visual" )] public string Description { get; set; } = "Empty description.";
		[Category( "Visual" )] public Color Color { get; set; } = Color.White;

		[Category( "Icon" ), ResourceType( "vmdl" )] public string Model { get; set; }
		[Category( "Icon" )] public Angles Angles { get; set; }
		[Category( "Icon" )] public Vector3 Position { get; set; }

		[Category( "Amount" )] public float MaxAmount { get; set; } = 1;
		[Category( "Amount" )] public AmountType AmountType { get; set; } = AmountType.Integer;

		[Category( "Other" )] public float Weight { get; set; } = 1;

		[HideInEditor, JsonIgnore] public Texture Icon { get; private set; }
		[HideInEditor, JsonIgnore] public TypeDescription Interactable { get; private set; }

		private IEnumerable<(TypeDescription type, ItemAttribute attribute)> types = null;

		protected override void PostLoad()
		{
			if ( !all.ContainsKey( ResourceName ) )
			{
				all.Add( ResourceName, this );

				if ( types == null )
					types = TypeLibrary.GetTypesWithAttribute<ItemAttribute>();

				var boundType = types.Where( tuple => tuple.attribute.Resource == ResourceName ).FirstOrDefault();
				if ( boundType != default )
					Interactable = boundType.type;
				
				if ( Host.IsServer ) return;

				var world = new SceneWorld();

				var model = Sandbox.Model.Load( Model ) 
					?? Sandbox.Model.Load( "models/dev/error.vmdl" );
				var obj = new SceneModel(
					world,
					model,
					new Transform( Position, Angles.ToRotation(), 1f ) );
				var light = new SceneLight( world, Vector3.Forward * 100f + Vector3.Up * 20f, 100f, Color.White * 0.5f );

				var camera = new SceneCamera( "itemIcon" )
				{
					World = world,
					Position = Vector3.Forward * 100f,
					Rotation = Rotation.From( 0, 180, 0 ),
					FieldOfView = 60f
				};
				var size = new Vector2( 256f, 256f );
				var render = camera.Render( size );

				Icon = render.Texture;
			}
		}
	}

	public ItemResource Resource { get; protected set; }

	/// <summary>
	/// Get ItemResource by name.
	/// </summary>
	/// <param name="resourceName"></param>
	/// <returns></returns>
	public static ItemResource Get( string resourceName )
	{
		if ( !ItemResource.All.TryGetValue( resourceName, out var resource ) )
			return null;

		return resource;
	}

	/// <summary>
	/// Get ItemResource by BaseInteractable type.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static ItemResource GetByType( Type type )
	{
		var resource = ItemResource.All.Values.Where( res => res?.Interactable?.ClassName == type.Name )
			.FirstOrDefault();

		return resource;
	}
}
