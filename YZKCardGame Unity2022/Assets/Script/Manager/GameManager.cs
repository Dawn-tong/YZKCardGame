using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	static GameObject gameManagerObject;
	public static GameObject GameManagerObject {
		get {
			if (gameManagerObject == null) {
				//防止代码热更新时导致gameManagerObject = null;
				gameManagerObject = GameObject.Find("Game Manager");
				if (gameManagerObject == null) {
					gameManagerObject = new GameObject("Game Manager");
					DontDestroyOnLoad(gameManagerObject);
				}
			}
			return gameManagerObject;
		}
	}

	delegate void InitDelegate();
	static List<InitDelegate> initActions = new List<InitDelegate>();
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void GameStart() {
		Debug.Log("Manager.GameStart - Init");
		initActions.Add(PlayerManager.Instance.Init);
		initActions.Add(NetManager.Instance.Init);
		initActions.Add(SceneLoaderManager.Instance.Init);
		initActions.Add(UIManager.Instance.Init);
		Debug.Log($"注册了 {initActions.Count} 个待初始化Manager");
		foreach (var initAction in initActions) {
			initAction();  // 直接调用
		}
	}

	static int finishCount = 0;
	public delegate void FinishInitDelegate();
	public static event FinishInitDelegate OnAllManagersFinishInit;
	public static void FinishInit() {
		finishCount++;
		if (finishCount == initActions.Count) {
			Debug.Log("所有Manager初始化完成");
			UIMessagePanel.Instance.AddMessage("初始化完成");
			OnAllManagersFinishInit?.Invoke();
		}
	}
}
