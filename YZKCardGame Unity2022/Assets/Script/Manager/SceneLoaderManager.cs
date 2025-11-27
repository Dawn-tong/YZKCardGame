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
	LoadScene,
	RoomScene,
	RuleScene,
	TitleScene,
}

public class SceneLoaderManager : ManagerBase<SceneLoaderManager> {
	Scene currentScene;
	public void Init()  {
		InitCurrentScene();
		GameManager.OnAllManagersFinishInit += OnAllManagersFinishInit;
		GameManager.FinishInit();
	}
	void InitCurrentScene() {
		// 进入当前场景
		if (Enum.TryParse(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, out Scene sceneValue)) {
			currentScene = sceneValue;
		}
		else {
			Debug.LogError("当前场景不在枚举值中");
			currentScene = Scene.None;
		}
	}
	void OnAllManagersFinishInit() {
		// 等待所有Manager运行结束后运行
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        SceneLoaderManager初始化回调        ————");
		GameManager.OnAllManagersFinishInit -= OnAllManagersFinishInit;
		
		initIsFinish = true;
		InvokeSceneEnter();
		
		Log.ReducePerfixLength();
	}
	//离开游戏自动运行当前场景离开
	void OnApplicationQuit() {
		InvokeSceneLeave();
		// 清理资源
		currentController = null;
		onProgress = null;
		Debug.Log($"{Log.perfix}SceneLoaderManager.OnApplicationQuit - 场景加载管理器清理完成");
	}
	
	
	
	
	
	
	bool initIsFinish = false;
	UnityAction<float> onProgress = null;
	/// <summary>
	/// 切换到指定场景
	/// </summary>
	public void LoadScene(Scene scene) {
		if (!initIsFinish) {
			Debug.LogWarning($"初始化未完成，请等待初始化完成后再切换场景");
			UIManager.Instance.CreateUI<UIMessage>().InitUIMessage("请稍后", "初始化中，请等待初始化完成后再切换场景");
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
		InvokeSceneLeave();
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
		InvokeSceneEnter();
		Log.ReducePerfixLength();
	}
	/// <summary>
	/// 设置加载进度回调
	/// </summary>
	public void SetProgressCallback(UnityAction<float> callback) {
		onProgress = callback;
	}
	
	
	
	
	
	
	SceneControllerBase currentController;
	void InvokeSceneEnter() {
		//调用代码注册的回调
		if (onSceneEnterCallbacks.ContainsKey(currentScene)) {
			onSceneEnterCallbacks[currentScene]?.Invoke();
		}
		//查找场景上的控制器
		var controllerObject = GameObject.Find("SceneController");
		if (controllerObject == null) {
			Debug.LogWarning($"{Log.perfix}场景 {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} 中未找到名为 SceneController 的物体");
			return;
		}
		//调用控制器回调
		currentController = controllerObject.GetComponent<SceneControllerBase>();
		if (currentController != null) {
			Debug.Log($"{Log.perfix}调用场景 {currentScene} 的进入回调");
			currentController.OnSceneEnter();
		}
		else {
			Debug.LogError($"{Log.perfix}物体 SceneController 上没有挂载 SceneControllerBase 派生类");
		}
	}
	void InvokeSceneLeave() {
		//调用代码注册的回调
		if (onSceneLeaveCallbacks.ContainsKey(currentScene)) {
			onSceneLeaveCallbacks[currentScene]?.Invoke();
		}
		//调用控制器回调
		if (currentController != null) {
			Debug.Log($"{Log.perfix}调用场景 {currentScene} 的离开回调");
			currentController.OnSceneLeave();
		}
		currentController = null;
	}
	
	
	
	
	
	
	//可以使用代码注册的回调
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
	public void UnregisterSceneEnterCallback(Scene scene, UnityAction callback) {
		if (onSceneEnterCallbacks.ContainsKey(scene)) {
			onSceneEnterCallbacks[scene] -= callback;
		}
	}
	public void UnregisterSceneLeaveCallback(Scene scene, UnityAction callback) {
		if (onSceneLeaveCallbacks.ContainsKey(scene)) {
			onSceneLeaveCallbacks[scene] -= callback;
		}
	}
}