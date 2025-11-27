using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FlickerFlash : MonoBehaviour {
	[SerializeField] float deltaTime = 0.6f; // 透明度变化持续时间（秒）
	Tween fadeTween; // 保存tween引用
	
	//创建DoTween动画
	void Start() {
		//创建DoTween动画：初始透明度为0，然后从0到1再从1到0循环播放
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null) {
			Color color = spriteRenderer.color;
			color.a = 0f;
			spriteRenderer.color = color;
			
			fadeTween = spriteRenderer.DOFade(0.5f, deltaTime)
				.SetEase(Ease.InSine)
				.SetLoops(-1, LoopType.Yoyo);
		}
	}
	
	void OnDestroy() {
		// 销毁时删除tween，避免内存泄漏
		if (fadeTween != null && fadeTween.IsActive()) {
			fadeTween.Kill();
		}
	}
}
