using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ManagerBase<UIManager> {
	//UI排序顺序
	public static readonly int messagePanelSortOrder = 9999; //消息面板
	public static readonly string messagePanelResourcePath = "Prefab/UI/Log Panel";
	//初始化
	public void Init(){
		InitAllCreatableUI();
		UIMessagePanel.Init();
		GameManager.FinishInit();
	}
	
	
	
	


	//初始化可被创建的UI
	class UIElement {
		public string resourcePath;
		public bool isSingleton;	//如果是单例
		public GameObject UIObject;
		public GameObject prefabCache;
	}
	Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();
	Dictionary<GameObject, UIElement> UIMapping = new Dictionary<GameObject, UIElement>();
	public void InitAllCreatableUI(){
		UIResources.Add(typeof(UIPopup), new UIElement() { resourcePath = "Prefab/UI/UIPopup", isSingleton = true });
		UIResources.Add(typeof(UIMessage), new UIElement() { resourcePath = "Prefab/UI/UIMessage", isSingleton = true });
	}






	//创建UI
	[SerializeField] int currentSortOrder = 10;
	public T CreateUI<T>() {
		//SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
		Type type = typeof(T);
		if (!UIResources.TryGetValue(type, out UIElement info)) {
			Debug.LogError($"无法找到 {type} 的 UIElement！请检查是否在 InitAllCreatableUI 中添加了。");
			return default(T);
		}
		//如果是单例
		if (info.isSingleton) {
			if (info.UIObject == null) {
				GameObject prefab = Resources.Load(info.resourcePath) as GameObject;
				if (prefab == null) {
					Debug.LogError($"无法加载 \"{info.resourcePath}\" prefab！请检查路径和文件名。");
					return default(T);
				}
				info.UIObject = Instantiate(prefab);
				DontDestroyOnLoad(info.UIObject);
				info.UIObject.transform.SetParent(UIManager.ManagerObj.transform);
				info.UIObject.GetComponent<Canvas>().sortingOrder = currentSortOrder++;
				UIMapping.Add(info.UIObject, info);
			}
			info.UIObject.SetActive(true);
			return info.UIObject.GetComponent<T>();
		}
		//不是单例的情况
		else {
			if (info.prefabCache == null) {
				info.prefabCache = Resources.Load(info.resourcePath) as GameObject;
				if (info.prefabCache == null) {
					Debug.LogError($"无法加载 {info.resourcePath} prefab！请检查路径和文件名。");
					return default(T);
				}
			}
			GameObject uiObject = Instantiate(info.prefabCache);
			uiObject.GetComponent<Canvas>().sortingOrder = currentSortOrder++;
			UIMapping.Add(uiObject, info);
			return uiObject.GetComponent<T>();
		}
	}






	//关闭UI
	public void CloseUI(GameObject uiObject) {
		if (UIMapping.TryGetValue(uiObject, out UIElement info)) {
			if (info.isSingleton) {
				uiObject.SetActive(false);
			}
			else {
				Destroy(uiObject);
				UIMapping.Remove(uiObject);
			}
		}
	}
}
