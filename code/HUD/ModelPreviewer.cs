namespace SpookyJam2022;


public struct ModelPreview
{
	public string Name;
	public string Path;
	public string Author;
	public Angles Rotation;
	public Vector3 Offset;
	public float Zoom;

	public static ModelPreview[] All => new ModelPreview[]
	{
		new ModelPreview { Name = "Polewik", Path = "models/polewik/polewik.vmdl", Author = "Grodbert", Rotation = new Angles( 20, 10, 0 ), Offset = new Vector3( 0, 0, -20 ), Zoom = 0.5f },
		new ModelPreview { Name = "Ammo Pack", Path = "models/items/ammo.vmdl", Author = "Luke", Rotation = new Angles( 20, 45, 0 ), Offset = new Vector3( 0, 0, -15 ), Zoom = 1.5f },
		new ModelPreview { Name = "Bear Trap", Path = "models/items/beartrap.vmdl", Author = "Luke", Rotation = new Angles( 35, 45, 0 ), Offset = new Vector3( 0, 0, 0 ), Zoom = 1.5f },
		new ModelPreview { Name = "Locked Chest", Path = "models/items/chest.vmdl", Author = "Luke", Rotation = new Angles( 0, -60, -20 ), Offset = new Vector3( 0, 0, -15 ), Zoom = 1.2f },
		new ModelPreview { Name = "Crowbar", Path = "models/items/crowbar.vmdl", Author = "Luke", Rotation = new Angles( 15, 45, 60 ), Offset = new Vector3( 0, -30, 10 ), Zoom = 2f },
		new ModelPreview { Name = "Key", Path = "models/items/key.vmdl", Author = "Luke", Rotation = new Angles( 0, 30, 180 ), Offset = new Vector3( 0, 0, 0 ), Zoom = 4f },
		new ModelPreview { Name = "Lock", Path = "models/items/lock.vmdl", Author = "Luke", Rotation = new Angles( 0, 235, 0 ), Offset = new Vector3( 0, 0, 15 ), Zoom = 5f },
		new ModelPreview { Name = "Medkit", Path = "models/items/medkit.vmdl", Author = "Luke", Rotation = new Angles( -20, 225, 0 ), Offset = new Vector3( 0, 0, -10 ), Zoom = 1.8f },
		new ModelPreview { Name = "Page", Path = "models/items/page.vmdl", Author = "Luke", Rotation = new Angles( 60, 20, 0 ), Offset = new Vector3( 0, 0, 0 ), Zoom = 2.6f },
		new ModelPreview { Name = "Sign", Path = "models/items/sign.vmdl", Author = "Luke", Rotation = new Angles( 0, 90, 0 ), Offset = new Vector3( 0, 0, -25 ), Zoom = 0.7f }
		/*
		["models/placeholders/placeholder_key.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_beartrap.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_crowbar.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_shotgun.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_medkit.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_page.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_ammo.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_locked_door.vmdl"] = "Placeholder (ubre)",
		["models/placeholders/placeholder_locked_chest.vmdl"] = "Placeholder (ubre)"*/
	};

}

class ModelPreviewer : Panel
{

	ModelViewer viewer;
	Label modelInfo;
	int currentSelected = 0;

	public ModelPreviewer()
	{

		viewer = new ModelViewer( Model.Load( ModelPreview.All[0].Path ), new Transform( ModelPreview.All[0].Offset, Rotation.From( ModelPreview.All[0].Rotation ), ModelPreview.All[0].Zoom ) );
		viewer.Style.Width = Length.Percent( 100 );
		viewer.Style.Height = Length.Percent( 100 );
		AddChild( viewer );

		modelInfo = AddChild<Label>( "ModelInfo" );
		modelInfo.SetText( $"{ModelPreview.All[0].Name} - {ModelPreview.All[0].Author}" );

	}

	[Event("NextPreview")]
	void nextPreview()
	{

		viewer.Delete();

		currentSelected = (currentSelected + 1) % ModelPreview.All.Count();
		var currentPreview = ModelPreview.All[currentSelected];

		viewer = new ModelViewer( Model.Load( currentPreview.Path ), new Transform( currentPreview.Offset, Rotation.From( currentPreview.Rotation ), currentPreview.Zoom ) );
		viewer.Style.Width = Length.Percent( 100 );
		viewer.Style.Height = Length.Percent( 100 );
		AddChild( viewer );

		modelInfo.Delete();
		modelInfo = AddChild<Label>( "ModelInfo" );
		modelInfo.SetText( $"{currentPreview.Name} - {currentPreview.Author}" );

	}

	[Event("PreviousPreview")]
	void previousPreview()
	{

		viewer.Delete();

		currentSelected = (currentSelected - 1) % ModelPreview.All.Count();
		var currentPreview = ModelPreview.All[currentSelected];

		viewer = new ModelViewer( Model.Load( currentPreview.Path ), new Transform( currentPreview.Offset, Rotation.From( currentPreview.Rotation ), currentPreview.Zoom ) );
		viewer.Style.Width = Length.Percent( 100 );
		viewer.Style.Height = Length.Percent( 100 );
		AddChild( viewer );

		modelInfo.Delete();
		modelInfo = AddChild<Label>( "ModelInfo" );
		modelInfo.SetText( $"{currentPreview.Name} - {currentPreview.Author}" );

	}

}
