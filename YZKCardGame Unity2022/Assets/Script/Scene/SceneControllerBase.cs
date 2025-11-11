using UnityEngine;

public abstract class SceneControllerBase : MonoBehaviour {
    /// <summary>
    /// 场景进入时的初始化
    /// </summary>
    public virtual void OnSceneEnter() { }
    
    /// <summary>
    /// 场景离开时的清理
    /// </summary>
    public virtual void OnSceneLeave() { }
}
