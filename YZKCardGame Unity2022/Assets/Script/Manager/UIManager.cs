using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class UIManager : ManagerBase<UIManager>
{
	//初始化永久面板
	public void Init(){
        CreateMessagePanel();
        GameManager.FinishInit();
    }

	//消息面板
	[SerializeField] int messagePanelSortOrder = 999;
    void CreateMessagePanel()
    {
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

    // TODO: 弹窗

}
