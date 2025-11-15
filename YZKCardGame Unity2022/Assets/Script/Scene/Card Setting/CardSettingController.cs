using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSettingController : SceneControllerBase {

	[SerializeField] CardSettingUI cardSettingUI;
	[SerializeField] CardSettingBoard cardSettingBoard;
	[SerializeField] CardSettingUI_OneCardPanel oneCardPanel;

	public override void OnSceneEnter() {
		cardSettingUI.OnSceneEnter();
		cardSettingBoard.OnSceneEnter();
		oneCardPanel.OnSceneEnter();
	}

	public override void OnSceneLeave() {
		cardSettingUI.OnSceneLeave();
		cardSettingBoard.OnSceneLeave();
	}
}
