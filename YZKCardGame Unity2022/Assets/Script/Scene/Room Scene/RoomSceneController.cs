using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSceneController : SceneControllerBase
{
	public override void OnSceneEnter() {
		RoomSceneService.Instance.OnSceneEnter();
	}

	public override void OnSceneLeave() {
		RoomSceneService.Instance.OnSceneLeave();
	}
}
