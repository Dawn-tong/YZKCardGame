using UnityEngine;
using UnityEngine.UI;

public class UIInput : UIBase {
    public GameObject goBackReigon;
    public Text titleText;
    public Text tipText;
    public InputField inputField;
    public Button confirmButton;
    public Button cancelButton;






    //关闭时回调
	public delegate void CloseDelegate(bool isConfirm, string result);
	public event CloseDelegate OnClose;
	public void ClickButtonToConfirm() {
		OnClose?.Invoke(true, inputField.text);
		UIManager.Instance.CloseUI(gameObject);
	}
	public void ClickButtonToCancel() {
		OnClose?.Invoke(false, null);
		UIManager.Instance.CloseUI(gameObject);
	}






    public void InitUIInput(string title, string tipText, CloseDelegate onClose = null) {
        titleText.text = title;
        this.tipText.text = tipText;
        OnClose = onClose;
    }
}
