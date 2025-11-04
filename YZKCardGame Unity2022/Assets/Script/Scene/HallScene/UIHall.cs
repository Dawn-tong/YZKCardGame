using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIHall : MonoBehaviour
{
	public Text titleText;
	public Button creatRoomButton;
	public Button joinRoomButton;
	public InputField joinNumberInputField;
	public Button returnTitleButton;

	public void ClickButtonToCreatRoom()
	{
		Debug.Log("Button - Creat Room");
		NetManager.Instance.isHostPlayer = true;
		SceneLoaderManager.Instance.LoadScene("RoomScene");
	}
	public void ClickButtonToJoinRoom()
	{
		Debug.Log("Button - Join Room");
		if (string.IsNullOrEmpty(joinNumberInputField.text))
		{
			Debug.LogError("加入码不能为空");
			UIMessagePanel.Instance.AddMessage($"加入码不能为空");
			return;
		}
		NetManager.Instance.isHostPlayer = false;
		NetManager.Instance.JoinRelayRoom(joinNumberInputField.text);
		SceneLoaderManager.Instance.LoadScene("RoomScene");
	}
	public void ClickButtonToReturnTitle()
	{
		Debug.Log("Button - Return To Title");
		SceneLoaderManager.Instance.LoadScene("TitleScene");
	}
}
