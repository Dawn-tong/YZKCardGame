using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : SceneControllerBase {

	[SerializeField] TitleSceneUI titleSceneUI;
	
	public override void OnSceneEnter() {
		titleSceneUI.OnSceneEnter();
	}

	public override void OnSceneLeave() { }
}
