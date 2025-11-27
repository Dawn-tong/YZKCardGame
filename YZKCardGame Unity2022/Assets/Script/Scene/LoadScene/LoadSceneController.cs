using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneController : SceneControllerBase {
	public override void OnSceneEnter() {
        SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
    }

	public override void OnSceneLeave() { }
}
