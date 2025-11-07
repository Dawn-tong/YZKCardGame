using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirdCell : MonoBehaviour
{
	public Action OnClick;
	void OnMouseDown()
	{
		OnClick?.Invoke();
	}
}
