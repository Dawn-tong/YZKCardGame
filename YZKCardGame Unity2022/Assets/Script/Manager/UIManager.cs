using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ManagerBase<UIManager> {
	//初始化永久面板
	public void Init(){
        CreateMessagePanel();
        GameManager.FinishInit();
    }






	//消息面板
	[SerializeField] int messagePanelSortOrder = 999;
    void CreateMessagePanel() {
		//创建CanvasObj
		GameObject messagePanelCanvasObj = new GameObject("messagePanelCanvas");
		DontDestroyOnLoad(messagePanelCanvasObj);
		messagePanelCanvasObj.transform.SetParent(ManagerObj.transform);
        //设置Canvas
		Canvas canvas = messagePanelCanvasObj.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.overrideSorting = true;
		canvas.sortingOrder = messagePanelSortOrder;
		CanvasScaler scaler = messagePanelCanvasObj.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		GraphicRaycaster raycaster = messagePanelCanvasObj.AddComponent<GraphicRaycaster>();
        //创建Panel
		GameObject messagePanelObj = new GameObject("messagePanelObj");
		DontDestroyOnLoad(messagePanelObj);
        messagePanelObj.transform.SetParent(messagePanelCanvasObj.transform);
        // 添加RectTransform
        RectTransform rootRect = messagePanelObj.AddComponent<RectTransform>();
        rootRect.SetParent(messagePanelObj.transform, false);
        rootRect.anchorMin = new Vector2(0, 0.01f);
        rootRect.anchorMax = new Vector2(0, 0.01f);
        rootRect.pivot = new Vector2(0, 0);
        rootRect.anchoredPosition = new Vector2(0, 0);
        rootRect.sizeDelta = new Vector2(500, 300);

		UIMessagePanel.Instance = messagePanelObj.AddComponent<UIMessagePanel>();
		UIMessagePanel.Instance.Init();
	}






    //TODO: 弹窗






    //UI屏蔽
    static readonly HashSet<UIShield> ActiveShields = new HashSet<UIShield>();
    public static void RegisterShield(UIShield shield) {
        if (shield == null) {
            return;
        }
        ActiveShields.Add(shield);
    }
    public static void UnregisterShield(UIShield shield) {
        if (shield == null) {
            return;
        }
        ActiveShields.Remove(shield);
    }
	/// <summary>
	/// 判断是否点击到存在UIShield组件的UI上
	/// </summary>
	public static bool IsClickBlockingUI() {
        ActiveShields.RemoveWhere(shield => shield == null);
        Vector2 pointer = Input.mousePosition;
        foreach (UIShield shield in ActiveShields) {
            RectTransform rectTransform = shield.RectTransform;
            if (rectTransform == null) {
                continue;
            }
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pointer, eventCamera)) {
                return true;
            }
        }
        return false;
	}
}
