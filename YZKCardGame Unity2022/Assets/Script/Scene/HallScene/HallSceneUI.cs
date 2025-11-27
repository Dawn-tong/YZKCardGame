using UnityEngine;
using UnityEngine.UI;

public class HallSceneUI : MonoBehaviour
{
	public Text titleText;
	public Button creatRoomButton;
	public Button joinRoomButton;
	public InputField joinNumberInputField;
	public Button goBackTitleButton;

	public void ClickButtonToCreatRoom() {
		Debug.Log("按钮 - 创建房间");
		NetManager.Instance.isHostPlayer = true;
		RoomCommunicateService.Instance.BeforeEnterRoom();
		NetManager.Instance.CreateRelayRoom(4);
		SceneLoaderManager.Instance.LoadScene(Scene.RoomScene);
	}
	public void ClickButtonToJoinRoom() {
		Debug.Log("按钮 - 加入房间");
		if (string.IsNullOrEmpty(joinNumberInputField.text)) {
			UIManager.Instance.CreateUI<UIMessage>().InitUIMessage("警告","加入码不能为空");
			return;
		}
		NetManager.Instance.isHostPlayer = false;
		RoomCommunicateService.Instance.BeforeEnterRoom();
		string joinCode = joinNumberInputField.text;
		//转换为全大写
		joinCode = joinCode.ToUpper();
		//忽略空格和换行符
		joinCode = joinCode.Replace(" ", "").Replace("\n", "");
		NetManager.Instance.JoinRelayRoom(joinCode);
		SceneLoaderManager.Instance.LoadScene(Scene.RoomScene);
	}
	public void ClickButtonToPasteRoomNumber() {
		string joinCode = GUIUtility.systemCopyBuffer;
		if (string.IsNullOrEmpty(joinCode)) {
			return;
		}
		//转换为全大写
		joinCode = joinCode.ToUpper();
		//忽略空格和换行符
		joinCode = joinCode.Replace(" ", "").Replace("\n", "");
		joinNumberInputField.text = joinCode;
	}
	public void ClickButtonToGoBackTitle() {
		Debug.Log("按钮 - 返回标题");
		SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
	}
}
