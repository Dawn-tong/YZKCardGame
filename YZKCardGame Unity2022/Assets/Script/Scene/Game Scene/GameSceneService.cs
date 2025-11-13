using Network;
using ProtoMessage;
using UnityEngine;

public class GameSceneService : MonoBehaviour {
    static GameSceneService instance;
	public static GameSceneService Instance {
		get {
			if (instance == null)
				instance = new GameSceneService();
			return instance;
		}
	}






    public void OnSceneEnter() {
        if (NetManager.Instance.isHostPlayer) {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.readyToStart = new ReadyToStartResponse();
        }
    }

    public void OnSceneLeave() {
    }
}
