using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneControllerBase
{
    // 自动生成场景名：类名去掉"Controller"后缀
    string sceneName;
    public virtual string SceneName {
        set {
            sceneName = value;
        }
        get {
            if (sceneName == null){
                sceneName = this.GetType().Name.Replace("Controller", "");
            }
            return sceneName;
        }
    }
    
    /// <summary>
    /// 场景进入时的初始化
    /// </summary>
    public virtual void OnSceneEnter() { }
    
    /// <summary>
    /// 场景离开时的清理
    /// </summary>
    public virtual void OnSceneLeave() { }
}
