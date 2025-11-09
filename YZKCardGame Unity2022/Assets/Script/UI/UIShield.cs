using UnityEngine;

public class UIShield : MonoBehaviour {
    RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform;
    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        UIManager.RegisterShield(this);
        if (rectTransform == null) {
            Debug.LogWarning($"{nameof(UIShield)}需要附加在含有RectTransform的对象上", this);
        }
    }
    void OnEnable() {
        UIManager.RegisterShield(this);
    }



	void OnDisable() {
        UIManager.UnregisterShield(this);
    }
    void OnDestroy() {
        UIManager.UnregisterShield(this);
    }
}
