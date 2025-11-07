using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Scene {
    None,
	CardSetting,
	GameScene,
	HallScene,
	RoomScene,
	TitleScene,
}

public class SceneLoaderManager : ManagerBase<SceneLoaderManager>
{
    Scene currentScene;
    Dictionary<Scene, SceneControllerBase> sceneControllers = new Dictionary<Scene, SceneControllerBase>();
    public void Init()  {
		// 注册场景控制器
		RegisterSceneController<CardSettingController>(Scene.CardSetting);
		RegisterSceneController<GameSceneController>(Scene.GameScene);
		RegisterSceneController<HallSceneController>(Scene.HallScene);
		RegisterSceneController<RoomSceneController>(Scene.RoomScene);
		RegisterSceneController<TitleSceneController>(Scene.TitleScene);
		// 进入当前场景
		if (Enum.TryParse(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, out Scene sceneValue)) {
			currentScene = sceneValue;
		}
		else {
            Debug.LogError("当前场景不在枚举值中");
			currentScene = Scene.None;
		}
        GameManager.OnAllManagersFinishInit += OnAllManagersFinishInit;
		GameManager.FinishInit();
    }
    void RegisterSceneController<T>(Scene scene) where T : SceneControllerBase, new() {
		var controller = new T();
        sceneControllers[scene] = controller;
	}

	void OnAllManagersFinishInit() {
		// 等待所有Manager运行结束后运行
        GameManager.OnAllManagersFinishInit -= OnAllManagersFinishInit;
		initIsFinish = true;
		if (sceneControllers.ContainsKey(currentScene)) {
            Debug.Log($"{Log.perfix}SceneLoaderManager.Init - 调用场景 {currentScene} 的进入回调");
			sceneControllers[currentScene].OnSceneEnter();
		}
		else {
			Debug.LogError($"{Log.perfix}SceneLoaderManager.Init - 场景控制器未注册: {currentScene}");
		}
        if (onSceneEnterCallbacks.ContainsKey(currentScene)) {
            Debug.Log($"{Log.perfix}SceneLoaderManager.Init - 调用其他系统触发的场景 {currentScene} 的进入回调");
            onSceneEnterCallbacks[currentScene]?.Invoke();
        }
	}
    //离开游戏自动运行当前场景离开
	void OnApplicationQuit() {
		if (sceneControllers.ContainsKey(currentScene)) {
			Debug.Log($"{Log.perfix}SceneLoaderManager.OnApplicationQuit - 调用场景 {currentScene} 的离开回调");
			sceneControllers[currentScene].OnSceneLeave();
		}
		else {
			Debug.LogWarning($"{Log.perfix}SceneLoaderManager.OnApplicationQuit - 场景 {currentScene} 未注册离开回调");
		}

		// 清理资源
		sceneControllers.Clear();
		onProgress = null;
		Debug.Log($"{Log.perfix}SceneLoaderManager.OnApplicationQuit - 场景加载管理器清理完成");
	}






	//其他系统触发的回调函数
	Dictionary<Scene, UnityAction> onSceneEnterCallbacks = new Dictionary<Scene, UnityAction>();
	Dictionary<Scene, UnityAction> onSceneLeaveCallbacks = new Dictionary<Scene, UnityAction>();
	public void RegisterSceneEnterCallback(Scene scene, UnityAction callback) {
        if (onSceneEnterCallbacks.ContainsKey(scene)) {
            onSceneEnterCallbacks[scene] += callback;
        }
        else {
            onSceneEnterCallbacks[scene] = callback;
        }
	}
	public void RegisterSceneLeaveCallback(Scene scene, UnityAction callback) {
        if (onSceneLeaveCallbacks.ContainsKey(scene)) {
            onSceneLeaveCallbacks[scene] += callback;
        }
        else {
            onSceneLeaveCallbacks[scene] = callback;
        }
	}





	bool initIsFinish = false;
	UnityAction<float> onProgress = null;
	/// <summary>
	/// 切换到指定场景
	/// </summary>
	public void LoadScene(Scene scene) {
        if (!initIsFinish) {
            Debug.LogWarning($"初始化未完成，请等待初始化完成后再切换场景");
			UIMessagePanel.Instance.AddMessage($"初始化未完成，请等待初始化完成后再切换场景");
            return;
		}
        if (currentScene == scene) {
            Debug.LogWarning("目标场景与当前场景名字相同");
            return;
        }
        StartCoroutine(LoadLevel(scene));
    }
	/// <summary>
	/// 协程：异步加载场景
	/// </summary>
	IEnumerator LoadLevel(Scene scene) {
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        SceneLoaderManager.LoadScene        ————");
		// 调用旧场景的离开回调
		if (sceneControllers.ContainsKey(currentScene)) {
            Debug.Log($"{Log.perfix}调用场景 {currentScene} 的离开回调");
            sceneControllers[currentScene].OnSceneLeave();
        }
        else {
            Debug.LogWarning($"{Log.perfix}场景 {currentScene} 未注册离开回调");
        }
        if (onSceneLeaveCallbacks.ContainsKey(currentScene)) {
            //Debug.Log($"{Log.perfix}调用其他系统触发的场景 {currentScene} 的离开回调");
            onSceneLeaveCallbacks[currentScene]?.Invoke();
        }

        // 加载新场景
		Debug.Log($"{Log.perfix}从场景 {currentScene} 切换到场景 {scene}");
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.ToString());
        async.allowSceneActivation = true;
        while (!async.isDone) {
            onProgress?.Invoke(async.progress);
            yield return null;
        }

        // 调用新场景的进入回调
        currentScene = scene;
        if (sceneControllers.ContainsKey(scene)) {
            Debug.Log($"{Log.perfix}调用场景 {scene} 的进入回调");
            sceneControllers[scene].OnSceneEnter();
        }
        else {
            Debug.LogWarning($"{Log.perfix}场景 {scene} 未注册进入回调");
        }
        if (onSceneEnterCallbacks.ContainsKey(scene)) {
            //Debug.Log($"{Log.perfix}调用其他系统触发的场景 {scene} 的进入回调");
            onSceneEnterCallbacks[scene]?.Invoke();
        }
        Log.ReducePerfixLength();
    }
    /// <summary>
    /// 设置加载进度回调
    /// </summary>
    public void SetProgressCallback(UnityAction<float> callback) {
        onProgress = callback;
    }
}