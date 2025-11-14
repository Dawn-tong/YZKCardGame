using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : UIBase {
    public GameObject goBackReigon;
    public RectTransform window;
	public float windowMinHeight = 450;
    public float windowMaxHeight = 850;
	public Text titleText;
	public RectTransform contentRectTransform; // 引用 Content 的 RectTransform
	public Text contentText;
	public Button confirmButton;
	public Button cancelButton;
    public Toggle noMoreTipToggle;
	
	
	
	
	
    //关闭时回调
	public delegate void CloseDelegate(bool result);
	public event CloseDelegate OnClose;
    //本次游戏不再提示
    public delegate void NoMoreTipDelegate(bool isOn);
    public event NoMoreTipDelegate OnNoMoreTip;
	public void ClickButtonToConfirm() {
		OnClose?.Invoke(true);
        OnNoMoreTip?.Invoke(noMoreTipToggle.isOn);
		window.anchoredPosition = new Vector2(0, 2000);
		UIManager.Instance.CloseUI(gameObject);
	}
	public void ClickButtonToCancel() {
		OnClose?.Invoke(false);
        OnNoMoreTip?.Invoke(noMoreTipToggle.isOn);
		window.anchoredPosition = new Vector2(0, 2000);
		UIManager.Instance.CloseUI(gameObject);
	}
	
	
	
	
	
	
	public void InitUIPopup(string title, string content, CloseDelegate onClose = null) {
        goBackReigon.SetActive(false);
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
		window.anchoredPosition = new Vector2(0, 0);
        goBackReigon.SetActive(true);
	}
    public void ShowNoMoreTip(NoMoreTipDelegate onNoMoreTip) {
        noMoreTipToggle.gameObject.SetActive(true);
        this.OnNoMoreTip = onNoMoreTip;
    }
}
