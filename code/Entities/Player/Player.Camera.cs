﻿namespace SpookyJam2022;

public partial class Player
{
	public override void PostCameraSetup( ref CameraSetup setup )
	{
		setup.Position = EyePosition;
		setup.Rotation = EyeRotation;
		setup.FieldOfView = 70f;
	}
}