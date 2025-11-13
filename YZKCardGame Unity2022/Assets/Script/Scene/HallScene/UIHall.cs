using UnityEngine;
using UnityEngine.UI;

public class UIHall : MonoBehaviour
{
	public Text titleText;
	public Button creatRoomButton;
	public Button joinRoomButton;
	public InputField joinNumberInputField;
	public Button goBackTitleButton;

	public void ClickButtonToCreatRoom() {
		Debug.Log("按钮 - 创建房间");
		NetManager.Instance.isHostPlayer = true;
		RoomService.Instance.BeforeEnterRoom();
		NetManager.Instance.CreateRelayRoom(4);
		SceneLoaderManager.Instance.LoadScene(Scene.RoomScene);
	}
	public void ClickButtonToJoinRoom() {
		Debug.Log("按钮 - 加入房间");
		if (string.IsNullOrEmpty(joinNumberInputField.text)) {
			Debug.LogError("加入码不能为空");
			UIMessagePanel.Instance.AddMessage($"加入码不能为空");
			return;
		}
		NetManager.Instance.isHostPlayer = false;
		RoomService.Instance.BeforeEnterRoom();
		NetManager.Instance.JoinRelayRoom(joinNumberInputField.text);
		SceneLoaderManager.Instance.LoadScene(Scene.RoomScene);
	}
	public void ClickButtonToGoBackTitle() {
		Debug.Log("按钮 - 返回标题");
		SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
	}
}
