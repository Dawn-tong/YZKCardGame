using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMessage : UIBase {
	public RectTransform window;
	public float windowMinHeight = 450;
	public float windowMaxHeight = 850;
	public Text titleText;
	public RectTransform contentRectTransform; // 引用 Content 的 RectTransform
	public Text contentText;
	public Button confirmButton;
	
	
	
	
	
    //关闭时回调
	public delegate void CloseDelegate();
	public event CloseDelegate OnClose;
	public void ClickButtonToConfirm() {
		OnClose?.Invoke();
		UIManager.Instance.CloseUI(gameObject);
	}
	public void ClickButtonToCancel() {
		OnClose?.Invoke();
		UIManager.Instance.CloseUI(gameObject);
	}
	
	
	
	
	
	
	public void InitUIMessage(string title, string content, CloseDelegate onClose = null) {
		titleText.text = title;
		contentText.text = content;
		this.OnClose = onClose;
		// 在下一帧调整 Content 大小，确保 Text 已经渲染
		StartCoroutine(AdjustContentSizeNextFrame());
	}
	IEnumerator AdjustContentSizeNextFrame() {
		yield return new WaitForEndOfFrame();
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(contentText.rectTransform);
		float textHeight = contentText.preferredHeight;
		contentText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);
		contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight + 20);
		//自动设置窗口高度
		window.sizeDelta = new Vector2(window.sizeDelta.x, Mathf.Clamp(textHeight + 320f, windowMinHeight, windowMaxHeight));
	}
}
