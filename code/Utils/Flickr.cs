using Sandbox;
using Sandbox.Internal;
using Sandbox.TextureLoader;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace CryptidHunt;

public static partial class Flickr
{
	public static readonly string[] LocalFallbacks =
	{
		"ui/fallbacks/pig.jpg",
		"ui/fallbacks/jerry.jpg",
		"ui/fallbacks/leha.png",
		"ui/fallbacks/seedy.jpg",
		"ui/fallbacks/snowball1.jpg",
		"ui/fallbacks/snowball2.png",
	};

	public static readonly int ResponsePrefixLength = "jsonFlickrFeed(".Length;
	public static readonly int ResponsePostfixLength = ")".Length;

	/// <summary>
	/// Time of "user patience" in seconds
	/// </summary>
	public static readonly float ImageLoadTimeout = 5.0f;

	class FlickPublicFeed
	{
		public class Item
		{
			[JsonPropertyName( "media" )]
			public Dictionary<string, string> Media { get; set; }
		}

		[JsonPropertyName( "items" )]
		public Item[] Items { get; set; }
	}

	private static async Task<string> GetUrl( string fear )
	{
		var jsonString = await Http.RequestStringAsync( $"https://www.flickr.com/services/feeds/photos_public.gne?format=json&tags={HttpUtility.UrlEncode( fear )}" );
		jsonString = jsonString.Substring( ResponsePrefixLength, jsonString.Length - ResponsePrefixLength - ResponsePostfixLength );

		FlickPublicFeed feed;
		try
		{
			feed = Json.Deserialize<FlickPublicFeed>( jsonString );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Got an exception while parsing Flickr JSON! ({ex}, the JSON in question: {jsonString})" );
			return default;
		}

		if ( feed.Items.Length == 0 )
			return default;

		var imageN = Random.Shared.Int( 0, Math.Min( 4, feed.Items.Length ) - 1 );
		var imageUrl = feed.Items[imageN].Media.FirstOrDefault().Value;

		if ( imageUrl != default )
			return imageUrl;

		return default;
	}

	private static async Task<Texture> GetFallback() => await Texture.LoadAsync( FileSystem.Mounted,
		LocalFallbacks[Random.Shared.Int( 0, LocalFallbacks.Length - 1 )] );

	public static async Task<Texture> Get( string fear )
	{
		var url = await GetUrl( fear );
		if ( url == default ) // got an empty url
			return await GetFallback();

		var image = Texture.Load( url );
		TimeSince imageStartedLoading = 0;
		while ( !image.IsLoaded )
		{
			await Task.Delay( 10 );

			if ( imageStartedLoading >= ImageLoadTimeout )
			{
				// fuck it
				image.Dispose();
				return await GetFallback();
			}
		}

		return image;
	}

	[ConCmd( "debug_get_fear" )]
	public static void DebugGetFear( string fear )
	{
		Log.Info( $"{GetUrl( fear ).Result}" );
	}
}
