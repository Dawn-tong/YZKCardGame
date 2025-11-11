using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSceneUI : MonoBehaviour {
	public void ClickButtonToGoBack(){
		SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
	}
}
