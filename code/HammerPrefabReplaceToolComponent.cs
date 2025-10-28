
using System;
using System.Text.Json.Serialization;

public sealed class HammerPrefabReplaceToolComponent : Component
{
	[Property]
	[JsonIgnore]
	[Category( "Debug" )]
	[Title( "Name to Replace" )]
	string replaceGameObjectsName { get; set; }

	[Property]
	[JsonIgnore]
	[Category( "Debug" )]
	[Title( "Prefab to Place" )]
	GameObject replaceWithPrefab { get; set; }

	[Button( "Find and Replace", "find_replace" ), Category( "Debug" )]
	public void DebugReplace()
	{
		if ( string.IsNullOrEmpty( replaceGameObjectsName ) || string.IsNullOrWhiteSpace( replaceGameObjectsName ) ) return;
		if ( !replaceWithPrefab.IsValid() ) return;

		var foundObjects = Scene.GetAllObjects( true )
			.Where( x => x.Name.Contains( replaceGameObjectsName, StringComparison.InvariantCultureIgnoreCase ) )
			.ToList();

		using ( Scene.Push() )
		{
			foreach ( var toReplace in foundObjects )
			{
				var spawnedPrefab = replaceWithPrefab.Clone();
				Log.Info( $"Replacing {toReplace} with {spawnedPrefab}." );

				spawnedPrefab.Transform.World = toReplace.Transform.World;
				spawnedPrefab.SetParent( toReplace.Parent );
				toReplace.Destroy();
			}
		}

		Log.Info( $"Replaced {foundObjects.Count()} GameObjects." );
	}
}
