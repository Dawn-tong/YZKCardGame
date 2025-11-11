using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSettingController : SceneControllerBase {

	[SerializeField] CardSettingBoard cardSettingBoard;
	[SerializeField] CardSettingUI_OneCardPanel oneCardPanel;

	public override void OnSceneEnter() {
		cardSettingBoard.OnSceneEnter();
		oneCardPanel.OnSceneEnter();
	}

	public override void OnSceneLeave() {
		cardSettingBoard.OnSceneLeave();
	}
}
