//UIText使用方法:
//通过UITextManager.CreateUIText()创建UI文本
//然后直接.SetXxxxx().SetXxxxx()即可设置UI文本(类似于DoTween的链式编程)
//示例:
//UITextManager.CreateUIText().SetName("示例文本").SetText("Hello World").SetFontSize(24).SetPosition(new Vector2(100, 100));
//即可创建一个 名为"示例文本"，显示"Hello World"，字体大小24，位置在(100,100) 的UI文本
//删除时直接调用uiText.Delete()即可
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UITextType
{
	UnityText,        //使用Unity自带的Text组件
	TextMeshPro       //使用TextMeshPro组件
}

public class UIText : MonoBehaviour
{
	public UITextType textType = UITextType.TextMeshPro;

	[SerializeField] string textName;
	[SerializeField] string textContent;
	[SerializeField] float fontSize = 14;
	[SerializeField] Vector2 position;
	[SerializeField] Color textColor = Color.white;

	Text unityText;
	TextMeshProUGUI tmpText;
	RectTransform rectTransform;

	Action<UIText> onDelete;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			GameObject textObj = new GameObject("TextObject");
			textObj.transform.SetParent(transform);
			rectTransform = textObj.AddComponent<RectTransform>();
		}
	}

	public UIText SetName(string name)
	{
		textName = name;
		if (rectTransform != null)
			rectTransform.gameObject.name = name;
		return this;
	}

	public UIText SetText(string content)
	{
		textContent = content;
		UpdateText();
		return this;
	}

	public UIText SetFontSize(float size)
	{
		fontSize = size;
		UpdateFontSize();
		return this;
	}

	public UIText SetPosition(Vector2 pos)
	{
		position = pos;
		UpdatePosition();
		return this;
	}

	public UIText SetColor(Color color)
	{
		textColor = color;
		UpdateColor();
		return this;
	}

	public UIText SetTextType(UITextType type)
	{
		textType = type;
		InitializeTextComponent();
		return this;
	}

	public UIText SetParent(Transform parent)
	{
		if (rectTransform != null)
			rectTransform.SetParent(parent, false);
		return this;
	}

	public UIText SetAnchoredPosition(Vector2 pos)
	{
		position = pos;
		if (rectTransform != null)
			rectTransform.anchoredPosition = pos;
		return this;
	}

	public UIText SetSizeDelta(Vector2 size)
	{
		if (rectTransform != null)
			rectTransform.sizeDelta = size;
		return this;
	}

	/// <summary>
	/// 删除时运行动作
	/// </summary>
	public UIText SubscribeDeleteDelegate(Action<UIText> action)
	{
		onDelete += action;
		return this;
	}

	public void Delete()
	{
		onDelete?.Invoke(this);
		onDelete = null;
	}

	private void InitializeTextComponent()
	{
		if (rectTransform == null)
			return;

		if (textType == UITextType.TextMeshPro)
		{
			if (tmpText == null)
			{
				if (unityText != null)
					Destroy(unityText);
				tmpText = rectTransform.gameObject.GetComponent<TextMeshProUGUI>();
				if (tmpText == null)
					tmpText = rectTransform.gameObject.AddComponent<TextMeshProUGUI>();
			}
		}
		else
		{
			if (unityText == null)
			{
				if (tmpText != null)
					Destroy(tmpText);
				unityText = rectTransform.gameObject.GetComponent<Text>();
				if (unityText == null)
					unityText = rectTransform.gameObject.AddComponent<Text>();
			}
		}

		UpdateText();
		UpdateFontSize();
		UpdatePosition();
		UpdateColor();
	}

	private void UpdateText()
	{
		if (tmpText != null)
			tmpText.text = textContent;
		else if (unityText != null)
			unityText.text = textContent;
	}

	private void UpdateFontSize()
	{
		if (tmpText != null)
			tmpText.fontSize = fontSize;
		else if (unityText != null)
			unityText.fontSize = (int)fontSize;
	}

	private void UpdatePosition()
	{
		if (rectTransform != null)
			rectTransform.anchoredPosition = position;
	}

	private void UpdateColor()
	{
		if (tmpText != null)
			tmpText.color = textColor;
		else if (unityText != null)
			unityText.color = textColor;
	}

	/// <summary>
	/// 手动初始化文本组件（在设置完所有属性后调用）
	/// </summary>
	public UIText Initialize()
	{
		InitializeTextComponent();
		return this;
	}
}

