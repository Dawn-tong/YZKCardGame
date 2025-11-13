using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : SceneControllerBase
{
	public override void OnSceneEnter() {
		GameSceneService.Instance.OnSceneEnter();
	}

	public override void OnSceneLeave() {
		GameSceneService.Instance.OnSceneLeave();
	}
}
