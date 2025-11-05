using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机拖拽管理器
/// 管理不同场景的摄像机拖拽控制器
/// </summary>
public class CameraDragManager : ManagerBase<CameraDragManager> {
    // 场景边界配置
    private Dictionary<string, SceneBoundsConfig> sceneBoundsConfigs = new Dictionary<string, SceneBoundsConfig>();
    public class SceneBoundsConfig {
        public Vector2 leftDownEdge;
        public Vector2 rightUpEdge;
        public SceneBoundsConfig(Vector2 leftDown, Vector2 rightUp) {
            leftDownEdge = leftDown;
            rightUpEdge = rightUp;
        }
    }
    //初始化
    public void Init() {
        Log.IncreasePerfixLength();
        Debug.Log($"{Log.perfix}————        CameraDragManager.Init        ————");
        RegisterSceneCallbacks<GameSceneController>(new Vector2(-20f, -15f), new Vector2(20f, 15f));
        RegisterSceneCallbacks<CardSettingController>(new Vector2(-15f, -12f), new Vector2(15f, 12f));
        Log.ReducePerfixLength();
        GameManager.FinishInit();
    }
    //注册场景回调（通过场景控制器类型自动获取场景名）
    void RegisterSceneCallbacks<T>(Vector2 leftDownEdge,Vector2 rightUpEdge) where T : SceneControllerBase, new() {
        // 通过创建临时实例获取场景名
        T tempController = new T();
        string sceneName = tempController.SceneName;
        Debug.Log($"{Log.perfix}设置场景{sceneName}的边界。左下=({leftDownEdge});右上=({rightUpEdge})");
        sceneBoundsConfigs.Add(sceneName, new SceneBoundsConfig(leftDownEdge, rightUpEdge));
        SceneLoaderManager.Instance.RegisterSceneEnterCallback(sceneName, () => OnSceneEnter(sceneName));
        SceneLoaderManager.Instance.RegisterSceneLeaveCallback(sceneName, () => OnSceneLeave(sceneName));
    }






	//当前场景的摄像机拖拽控制器
	[SerializeField] CameraDragController currentController;
    //场景进入回调(只有注册了场景回调的场景才会进入)
    void OnSceneEnter(string sceneName) {
        Log.IncreasePerfixLength();
        Debug.Log($"{Log.perfix}————        CameraDragManager.进入场景        ————");
        // 查找当前场景的主摄像机
        Camera mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogWarning($"{Log.perfix}场景{sceneName}中未找到主摄像机");
            Log.ReducePerfixLength();
            return;
        }
        // 获取或添加CameraDragController组件
        currentController = mainCamera.GetComponent<CameraDragController>();
        if (currentController == null) {
            currentController = mainCamera.gameObject.AddComponent<CameraDragController>();
            //Debug.Log($"{Log.perfix}为场景{sceneName}的摄像机添加了CameraDragController组件");
        }
        // 应用场景边界配置
        if (sceneBoundsConfigs.ContainsKey(sceneName)) {
            var config = sceneBoundsConfigs[sceneName];
            currentController.SetBounds(config.leftDownEdge, config.rightUpEdge);
            currentController.enabled = true;
            Debug.Log($"{Log.perfix}开启场景{sceneName}的摄像机拖拽。");
        }
        Log.ReducePerfixLength();
    }
    //场景离开回调(只有注册了场景回调的场景才会离开)
    void OnSceneLeave(string sceneName) {
        Log.IncreasePerfixLength();
        Debug.Log($"{Log.perfix}————        CameraDragManager.离开场景        ————");
        // 禁用当前控制器
        if (currentController != null) {
            Debug.Log($"{Log.perfix}关闭场景{sceneName}的摄像机拖拽。");
            currentController.enabled = false;
        }
        currentController = null;
        Log.ReducePerfixLength();
    }
}

