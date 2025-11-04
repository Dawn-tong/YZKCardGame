//UIText使用方法:
//通过CreateUIText()创建UI文本
//然后直接.SetXxxxx().SetXxxxx()即可设置UI文本(类似于DoTween的链式编程)
//示例:
//UITextManager.CreateUIText().SetName("示例文本").SetText("Hello World").SetFontSize(24).SetPosition(new Vector2(100, 100)).Initialize();
//即可创建一个 名为"示例文本"，显示"Hello World"，字体大小24，位置在(100,100) 的UI文本
//删除时直接调用uiText.Delete()即可
//注意：创建后需要调用.Initialize()来初始化文本组件
using System.Collections.Generic;
using UnityEngine;

public class UITextManager : ManagerBase<UITextManager>
{
	//UI文本列表
	public static List<UIText> uiTexts = new List<UIText>();

	/// <summary>
	/// 创建一个新的UIText
	/// </summary>
	public static UIText CreateUIText()
	{
		UIText uiText = ManagerObj.AddComponent<UIText>();
		uiText.SubscribeDeleteDelegate(RemoveUITextFromList);
		uiTexts.Add(uiText);
		return uiText;
	}

	/// <summary>
	/// 从列表中移除UIText并销毁
	/// </summary>
	private static void RemoveUITextFromList(UIText uiText)
	{
		uiTexts.Remove(uiText);
		if (uiText != null)
			Destroy(uiText.gameObject);
	}

	/// <summary>
	/// 根据名称查找UIText
	/// </summary>
	public static UIText FindUITextByName(string name)
	{
		foreach (var uiText in uiTexts)
		{
			if (uiText != null && uiText.name == name)
				return uiText;
		}
		return null;
	}

	/// <summary>
	/// 删除所有UIText
	/// </summary>
	public static void ClearAllUITexts()
	{
		for (int i = uiTexts.Count - 1; i >= 0; i--)
		{
			if (uiTexts[i] != null)
				uiTexts[i].Delete();
		}
		uiTexts.Clear();
	}
}

