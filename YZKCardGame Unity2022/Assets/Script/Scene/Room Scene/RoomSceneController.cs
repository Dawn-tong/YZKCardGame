using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSceneController : SceneControllerBase
{
	UIRoom UI;
	public override void OnSceneEnter()
	{
		UI = Object.FindFirstObjectByType<UIRoom>();
		UI.OnSceneEnter();
		RoomService.Instance.OnSceneEnter();
	}

	public override void OnSceneLeave()
	{
		UI.OnSceneLeave();
		RoomService.Instance.OnSceneLeave();
	}
}
