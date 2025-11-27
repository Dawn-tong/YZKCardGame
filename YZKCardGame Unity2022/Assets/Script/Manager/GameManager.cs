using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
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
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        Manager初始化        ————");
		initActions.Add(PlayerManager.Instance.Init);
		initActions.Add(NetManager.Instance.Init);
		initActions.Add(SceneLoaderManager.Instance.Init);//需要在CameraDragManager之前
		initActions.Add(CameraDragManager.Instance.Init);//需要在SceneLoaderManager之后
		initActions.Add(UIManager.Instance.Init);
		initActions.Add(StorageManager.Instance.Init);
		Debug.Log($"{Log.perfix}注册了 {initActions.Count} 个待初始化Manager");
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
			Debug.Log($"{Log.perfix}所有Manager初始化完成,运行回调函数");
			//UIMessagePanel.Instance.AddMessage("初始化完成");
			OnAllManagersFinishInit?.Invoke();
			Log.ReducePerfixLength();
			ServiceInit();
		}
	}






	static void ServiceInit() {
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        Service初始化        ————");
		Log.ReducePerfixLength();
	}
}
