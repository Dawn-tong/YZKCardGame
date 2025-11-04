using UnityEngine;
public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
{
	static T instance;
	public static T Instance {
		get {
			if (instance == null) {
				instance = ManagerObj.AddComponent<T>();
			}
			return instance; 
		}
	}
	static GameObject managerObj;
	public static GameObject ManagerObj {
		get {
			if (managerObj == null) {
				managerObj = new GameObject(typeof(T).Name);
				DontDestroyOnLoad(managerObj);
				managerObj.transform.SetParent(GameManager.GameManagerObject.transform);
			}
			return managerObj; 
		}
	}
}