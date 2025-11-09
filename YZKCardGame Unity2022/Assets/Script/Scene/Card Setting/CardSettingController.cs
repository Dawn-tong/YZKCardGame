using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSettingController : SceneControllerBase
{
	CardSettingUI UI;
	public override void OnSceneEnter() {
		UI = Object.FindFirstObjectByType<CardSettingUI>();
		UI.OnSceneEnter();
	}

	public override void OnSceneLeave() {
		UI.OnSceneLeave();
	}
}
