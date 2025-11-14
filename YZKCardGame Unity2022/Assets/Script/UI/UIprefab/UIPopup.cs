using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : UIBase {
    public Text TitleText;
    public RectTransform contentRectTransform; // 引用 Content 的 RectTransform
    public Text ContentText;
    public Button ConfirmButton;
    public Button CancelButton;





    public delegate void CloseDelegate(bool result);
    public event CloseDelegate OnClose;
    public void ClickButtonToConfirm() {
        OnClose?.Invoke(true);
        UIManager.Instance.CloseUI(gameObject);
    }
    public void ClickButtonToCancel() {
        OnClose?.Invoke(false);
        UIManager.Instance.CloseUI(gameObject);
    }






    public void InitUIPopup(string title, string content, CloseDelegate onClose) {
        TitleText.text = title;
        ContentText.text = content;
        this.OnClose = onClose;
        // 在下一帧调整 Content 大小，确保 Text 已经渲染
        StartCoroutine(AdjustContentSizeNextFrame());
    }
    IEnumerator AdjustContentSizeNextFrame() {
        yield return null; // 等待下一帧
        if (ContentText != null && contentRectTransform != null) {
            // 强制立即重建布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContentText.rectTransform);
            // 获取 Text 的首选高度
            float preferredHeight = ContentText.preferredHeight;
            float preferredWidth = ContentText.preferredWidth;
            // 设置 Content 的大小
            contentRectTransform.sizeDelta = new Vector2(
                contentRectTransform.sizeDelta.x, 
                preferredHeight
            );
        }
    }
}
