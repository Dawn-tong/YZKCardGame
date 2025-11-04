using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneLoaderManager : ManagerBase<SceneLoaderManager>
{
    string currentSceneName;
    Dictionary<string, SceneControllerBase> sceneControllers = new Dictionary<string, SceneControllerBase>();
    UnityAction<float> onProgress = null;

    bool initIsFinish = false;
    public void Init() 
    {
		// 注册场景控制器
		RegisterSceneController<CardSettingSceneController>();
		RegisterSceneController<GameSceneController>();
		RegisterSceneController<HallSceneController>();
		RegisterSceneController<RoomSceneController>();
		RegisterSceneController<TitleSceneController>();
		// 进入当前场景
		currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameManager.OnAllManagersFinishInit += OnAllManagersFinishInit;
		GameManager.FinishInit();

    }
    //注册场景控制器（自动获取场景名）
    void RegisterSceneController<T>() where T : SceneControllerBase, new()
    {
        // Debug.Log($"注册场景控制器: {controller.SceneName}");
        var controller = new T();
        sceneControllers[controller.SceneName] = controller;
    }
    void OnAllManagersFinishInit() {
		// 等待所有Manager运行结束后运行
        GameManager.OnAllManagersFinishInit -= OnAllManagersFinishInit;
		initIsFinish = true;
		if (sceneControllers.ContainsKey(currentSceneName))
		{
			sceneControllers[currentSceneName].OnSceneEnter();
		}
		else
		{
			Debug.LogError($"SceneLoaderManager.Init - 场景控制器未注册: {currentSceneName}");
		}
	}

    //离开游戏自动运行当前场景离开
	void OnApplicationQuit()
	{
		if (sceneControllers.ContainsKey(currentSceneName))
		{
			Debug.Log($"SceneLoaderManager.OnApplicationQuit - 调用场景 {currentSceneName} 的离开回调");
			sceneControllers[currentSceneName].OnSceneLeave();
		}
		else
		{
			Debug.LogWarning($"SceneLoaderManager.OnApplicationQuit - 场景 {currentSceneName} 未注册离开回调");
		}

		// 清理资源
		sceneControllers.Clear();
		onProgress = null;
		Debug.Log("SceneLoaderManager.OnApplicationQuit - 场景加载管理器清理完成");
	}

	/// <summary>
	/// 切换到指定场景
	/// </summary>
	public void LoadScene(string sceneName)
    {
        if (!initIsFinish) {
            Debug.LogWarning($"初始化未完成，请等待初始化完成后再切换场景");
			UIMessagePanel.Instance.AddMessage($"初始化未完成，请等待初始化完成后再切换场景");
            return;
		}
        if (currentSceneName == sceneName) {
            Debug.LogWarning("目标场景与当前场景名字相同");
            return;
        }
        Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        SceneLoaderManager.LoadScene        ————");
		Debug.Log($"{Log.perfix}从场景 {currentSceneName} 切换到场景 {sceneName}");
        StartCoroutine(LoadLevel(sceneName));
    }
    /// <summary>
    /// 协程：异步加载场景
    /// </summary>
    IEnumerator LoadLevel(string sceneName)
    {
        // 调用旧场景的离开回调
        if (sceneControllers.ContainsKey(currentSceneName))
        {
            Debug.Log($"{Log.perfix}调用场景 {currentSceneName} 的离开回调");
            sceneControllers[currentSceneName].OnSceneLeave();
        }
        else
        {
            Debug.LogWarning($"{Log.perfix}场景 {currentSceneName} 未注册离开回调");
        }

        // 加载新场景
        Debug.Log($"{Log.perfix}加载场景: {sceneName}");
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = true;
        
        while (!async.isDone)
        {
            onProgress?.Invoke(async.progress);
            yield return null;
        }

        // 调用新场景的进入回调
        currentSceneName = sceneName;
        if (sceneControllers.ContainsKey(sceneName))
        {
            Debug.Log($"{Log.perfix}调用场景 {sceneName} 的进入回调");
            sceneControllers[sceneName].OnSceneEnter();
        }
        else
        {
            Debug.LogWarning($"{Log.perfix}场景 {sceneName} 未注册进入回调");
        }
        
        Debug.Log($"{Log.perfix}场景切换完成: {sceneName}");
        Log.ReducePerfixLength();
    }
    /// <summary>
    /// 设置加载进度回调
    /// </summary>
    public void SetProgressCallback(UnityAction<float> callback)
    {
        onProgress = callback;
    }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    /// <summary>
    /// 获取指定场景的控制器
    /// </summary>
    public T GetSceneController<T>(string sceneName) where T : SceneControllerBase
    {
        if (sceneControllers.ContainsKey(sceneName))
        {
            return sceneControllers[sceneName] as T;
        }
        return null;
    }
}