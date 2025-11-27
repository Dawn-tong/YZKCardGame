using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : ManagerBase<StorageManager> {
	public void Init() {
		//加载玩家数据
		PlayerDataStorage.Init();
		//加载玩家卡组
		CardsStorage.Init();
		GameManager.FinishInit();
	}

    // 应用暂停时保存
	void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			PlayerPrefs.Save(); 
		}
	}
    // 应用退出时保存
	void OnApplicationQuit() {
		PlayerPrefs.Save();
	}
}
