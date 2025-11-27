using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMessagePanel : MonoBehaviour {
	public static UIMessagePanel Instance { get; set; }

	private GameObject ScrollView;
    private GameObject messageTextObj;
	private Transform content;
	private Button toggleButton;
    private Text buttonText;
    private bool isPanelVisible = true;
    
    public static void Init() {
		//创建CanvasObj
		GameObject messagePanelCanvasObj = new GameObject("messagePanelCanvas");
		DontDestroyOnLoad(messagePanelCanvasObj);
		messagePanelCanvasObj.transform.SetParent(UIManager.ManagerObj.transform);
		//设置Canvas
		Canvas canvas = messagePanelCanvasObj.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.overrideSorting = true;
		canvas.sortingOrder = UIManager.messagePanelSortOrder;
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
		
		Instance = messagePanelObj.AddComponent<UIMessagePanel>();
        //创建开关按钮
        Instance.CreateToggleButton();
        //创建滚动显示面板
        Instance.CreateScrollPanel();
        Instance.SetPanelVisible(false);
	}
    


    private void CreateToggleButton() {
        // 创建按钮对象
        GameObject buttonObj = new GameObject("ToggleButton");
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.SetParent(transform, false);
        buttonRect.anchorMin = new Vector2(0, 0);  // 左下角
        buttonRect.anchorMax = new Vector2(0, 1);  // 左上角
        buttonRect.pivot = new Vector2(0, 0.5f);   // 轴心点在左侧
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(60, 0);
		// 添加UIShield组件
		UIShield uIShield = buttonObj.AddComponent<UIShield>();
		// 添加Image组件
		Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        // 添加Button组件
        toggleButton = buttonObj.AddComponent<Button>();
        toggleButton.onClick.AddListener(() => {
            SetPanelVisible(!isPanelVisible); 
			if (!isPanelVisible) {
				ClearMessage();
			}
        });
        // 创建按钮物体
        GameObject buttonTextObj = new GameObject("ButtonText");
        RectTransform textRect = buttonTextObj.AddComponent<RectTransform>();
        textRect.SetParent(buttonObj.transform, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
		// 创建按钮文字
		buttonText = buttonTextObj.AddComponent<Text>();
		buttonText.text = "打\n开\n消\n息";
		buttonText.fontSize = 35;
		buttonText.color = Color.white;
		buttonText.alignment = TextAnchor.MiddleCenter;
		buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
	}
	public void SetPanelVisible(bool visible) {
		isPanelVisible = visible;
		if (isPanelVisible) {
			buttonText.text = "关\n闭\n消\n息";
		}
		else {
			buttonText.text = "打\n开\n消\n息";
		}
		if (ScrollView != null) {
			ScrollView.SetActive(visible);
		}
	}



	private void CreateScrollPanel() {
		//加载滚动条
		ScrollView = Resources.Load<GameObject>(UIManager.messagePanelResourcePath);
		if (ScrollView == null) {
			Debug.LogError($"无法加载 {UIManager.messagePanelResourcePath} prefab！请检查路径和文件名。");
			return;
		}	
		ScrollView = Instantiate(ScrollView);
		ScrollView.transform.SetParent(transform); // 设置为当前物体的子级
		RectTransform scrollRectTransform = ScrollView.GetComponent<RectTransform>();
		scrollRectTransform.SetParent(transform, false);
		// 设置位置（在按钮右侧）
		scrollRectTransform.anchorMin = new Vector2(0, 0);
		scrollRectTransform.anchorMax = new Vector2(1, 1);
		scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
		scrollRectTransform.offsetMin = new Vector2(60, 0);   // 左侧留空60像素给按钮
		scrollRectTransform.offsetMax = new Vector2(0, 0);    // 右侧不留空
		// 添加UIShield组件
		UIShield uIShield = ScrollView.AddComponent<UIShield>();

		// 创建消息文本
		messageTextObj = new GameObject("MessageText");
		RectTransform textRect = messageTextObj.AddComponent<RectTransform>();
		textRect.anchorMin = new Vector2(0, 1);
		textRect.anchorMax = new Vector2(1, 1);
		textRect.pivot = new Vector2(0.5f, 1);
		textRect.anchoredPosition = Vector2.zero;
		textRect.sizeDelta = new Vector2(-10, 100);
		//添加预设消息
		Text messageText = messageTextObj.AddComponent<Text>();
		messageText.fontSize = 30;
		messageText.color = Color.white;
		messageText.alignment = TextAnchor.UpperLeft;
		messageText.text = "";
		messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		//设置消息父节点
		content = ScrollView.GetComponent<ScrollRect>().content.transform;
		textRect.SetParent(content, false);
		//设置初始不显示
		messageTextObj.SetActive(false);
	}

    public void AddMessage(string message) {
		GameObject textObj = Instantiate(messageTextObj, content);
		textObj.GetComponent<Text>().text = DateTime.Now.ToString("[HH:mm:ss]") + message;
		textObj.SetActive(true);
		// 自动滚动到底部（下一帧，等待布局更新）
		StartCoroutine(ScrollToBottomNextFrame());
	}

	IEnumerator ScrollToBottomNextFrame() {
		yield return null; // 等待一帧，确保布局与 ContentSizeFitter/VerticalLayoutGroup 已更新
		if (ScrollView == null) {
			yield break;
		}
		Canvas.ForceUpdateCanvases();
		ScrollRect scrollRect = ScrollView.GetComponent<ScrollRect>();
		if (scrollRect != null) {
			// 强制重建 Content 布局，避免内容高度未更新导致无法滚动到底
			if (content is RectTransform rt) {
				LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
			}
			scrollRect.verticalNormalizedPosition = 0f;
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}
	public void ClearMessage() {
		if (content == null) {
			return;
		}
		for (int i = content.childCount - 1; i >= 0; i--) {
			Transform child = content.GetChild(i);
			if (messageTextObj != null && child.gameObject == messageTextObj) {
				// 保留模板文本对象但清空并隐藏
				Text templateText = child.GetComponent<Text>();
				if (templateText != null) {
					templateText.text = "";
				}
				child.gameObject.SetActive(false);
				continue;
			}
			Destroy(child.gameObject);
		}
		// 重置滚动位置到顶部
		if (ScrollView != null) {
			ScrollRect scrollRect = ScrollView.GetComponent<ScrollRect>();
			if (scrollRect != null) {
				scrollRect.verticalNormalizedPosition = 1f;
			}
		}
	}
}
