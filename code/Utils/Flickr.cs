using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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

	public static readonly string[] TagStopList =
	{
		"nsfw",
		"erotic",
		"sex",
		"nude",
		"naked",
		"porn",
		"gore",
		"dick",
		"penis",
		"pussy",
		"webcam",
		"masturb", // includes "masturbation"
		"fetish",
		"kink",
		"mature",
		"erection",
		"bdsm",
		"boobs",
		"tits"
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
			[JsonPropertyName( "tags" )]
			public string Tags { get; set; }
		}

		[JsonPropertyName( "items" )]
		public Item[] Items { get; set; }
	}

	private static async Task<string> GetUrl( string fear )
	{
		fear = Regex.Replace( fear.Trim(), @"\s+", " " );
		// rndtrash: feeds accept a comma separated list of tags. Comment the next line if the results are actually worse.
		fear = fear.Replace( " ", "," );
		var safeFear = HttpUtility.UrlEncode( fear );

		string jsonString;
		try
		{
			jsonString = await Http.RequestStringAsync( $"https://www.flickr.com/services/feeds/photos_public.gne?format=json&tags={safeFear}" );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Got an exception while reaching out to Flickr API! ({ex})" );
			return null;
		}
		jsonString = jsonString.Substring( ResponsePrefixLength, jsonString.Length - ResponsePrefixLength - ResponsePostfixLength );

		FlickPublicFeed feed;
		try
		{
			feed = Json.Deserialize<FlickPublicFeed>( jsonString );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Got an exception while parsing Flickr JSON! ({ex}, the JSON in question: {jsonString})" );
			return null;
		}

		var feedItems = feed.Items.Where( item => !TagStopList.Any( tag => item.Tags.Contains( tag ) ) ).ToList();
		if ( feedItems.Count == 0 )
		{
			// Oh. It's all porn. Bailing out!
			return null;
		}

		var item = Random.Shared.FromList( feedItems );
		Log.Info( $"Chose one with the following tags: {item.Tags}" );
				return item.Media.FirstOrDefault().Value;
			}

	private static async Task<Texture> GetFallback() => await Texture.LoadFromFileSystemAsync(
		LocalFallbacks[Random.Shared.Int( 0, LocalFallbacks.Length - 1 )], FileSystem.Mounted );

	public static async Task<Texture> Get( string fear )
	{
		var url = await GetUrl( fear );
		if ( url == null ) // got an empty url
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
