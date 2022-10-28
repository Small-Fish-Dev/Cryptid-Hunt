using Sandbox.Internal;
using System.Web;

namespace SpookyJam2022.Utils;

public static partial class Flickr
{
	public static readonly int ResponsePrefixLength = "jsonFlickrFeed(".Length;
	public static readonly int ResponsePostfixLength = ")".Length;

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

	static async Task<string> GetUrl( string fear )
	{
		var client = new Http( new( $"https://www.flickr.com/services/feeds/photos_public.gne?format=json&tags=scary,{HttpUtility.UrlEncode( fear )},horror" ) );

		var jsonString = await client.GetStringAsync();
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

		if ( feed.Items.Length == 0 )
			return null;

		var imageN = Random.Shared.Int( 0, Math.Min( 4, feed.Items.Length ) - 1 );
		var imageUrl = feed.Items[imageN].Media.FirstOrDefault().Value;
		return imageUrl;
	}

	public static async Task<Texture> Get( string fear )
	{
		string url = await GetUrl( fear );
		if ( url == default ) // got an empty url
			return Texture.Load( FileSystem.Mounted, "materials/pig/pig.jpg" );
		
		var client = new Http( new( url ) );

		var image = Sandbox.TextureLoader.Image.Load( await client.GetStreamAsync() );
		while ( !image.IsLoaded )
		{
			await Task.Delay( 10 );
		} // FIXME: copy pasted from the load code. should it really be there?

		return image;
	}

	[ConCmd.Admin( "debug_get_fear" )]
	public static void DebugGetFear( string fear )
	{
		Log.Info( $"{GetUrl( fear ).Result}" );
	}
}
