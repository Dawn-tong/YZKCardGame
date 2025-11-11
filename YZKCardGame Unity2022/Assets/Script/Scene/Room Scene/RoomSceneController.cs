using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSceneController : SceneControllerBase {

	[SerializeField] RoomSceneUI roomSceneUI;

	public override void OnSceneEnter() {
		roomSceneUI.OnSceneEnter();
		RoomSceneService.Instance.OnSceneEnter();
	}

	public override void OnSceneLeave() {
		roomSceneUI.OnSceneLeave();
		RoomSceneService.Instance.OnSceneLeave();
	}
}
