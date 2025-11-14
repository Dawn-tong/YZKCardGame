using System.Collections.Generic;
using UnityEngine;

public class UIShield : MonoBehaviour {
    RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform;
    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) {
            Debug.LogWarning($"{nameof(UIShield)}需要附加在含有RectTransform的对象上", this);
        }
		ActiveShields.Add(this);
    }
    void OnEnable() {
		ActiveShields.Add(this);
    }
	void OnDisable() {
		ActiveShields.Remove(this);
	}
    void OnDestroy() {
		ActiveShields.Remove(this);
	}






	static readonly HashSet<UIShield> ActiveShields = new HashSet<UIShield>();
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
