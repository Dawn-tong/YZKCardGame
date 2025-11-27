using System;
using System.IO;
using UnityEngine;

public static class CardsStorage {
	public static readonly int cardsGroupPresetCount = 5;
	public static int CurrentCardSetIndex { get; private set; } = 0;
	public static CardsListManager currentPlayerCardManager;
	const string CardSetIndexPrefsKey = "CardsStorage.CurrentCardSetIndex";
	public static void Init() {
		InitializeCardSetStorage();
		currentPlayerCardManager = PlayerManager.Instance.currentPlayer.cardManager;
		CurrentCardSetIndex = LoadCurrentCardSetIndex();
		LoadCardsListFromLocal(CurrentCardSetIndex);
	}






	static string cardListDirectory;    //本地卡组保存目录
	static string[] cardSetPaths;       //本地各卡组文件路径
	//初始化卡组存储
	public static void InitializeCardSetStorage() {
		cardListDirectory = Path.Combine(Application.persistentDataPath, "CardSets");
		if (!Directory.Exists(cardListDirectory)) {
			Directory.CreateDirectory(cardListDirectory);
		}
		if (cardSetPaths == null || cardSetPaths.Length != cardsGroupPresetCount) {
			cardSetPaths = new string[cardsGroupPresetCount];
		}
		for (int i = 0; i < cardsGroupPresetCount; i++) {
			cardSetPaths[i] = Path.Combine(cardListDirectory, $"CardList_{i}.json");
		}
	}






	//卡组下标
	static void SaveCurrentCardSetIndex(int index) {
		PlayerPrefs.SetInt(CardSetIndexPrefsKey, index);
		PlayerPrefs.Save();
	}
	static int LoadCurrentCardSetIndex() {
		int index = PlayerPrefs.GetInt(CardSetIndexPrefsKey, 0);
		if (index < 0 || index >= cardsGroupPresetCount) {
			return 0;
		}
		return index;
	}






	/// <summary>
	/// 保存卡组到本地
	/// </summary>
	public static void SaveCardsListToLocal() {
		int setIndex = CurrentCardSetIndex;
		try {
			CardListWrapper wrapper = new CardListWrapper {
				cards = BuildSerializableCards()
			};
			string json = JsonUtility.ToJson(wrapper, true);
			string targetPath = cardSetPaths[setIndex];
			File.WriteAllText(targetPath, json);
			Debug.Log($"卡组 {setIndex} 已保存到: {targetPath}");
			SaveCurrentCardSetIndex(setIndex);
		}
		catch (System.Exception e) {
			Debug.LogError($"保存卡组失败: {e.Message}");
		}
	}
	// 将数组包装成可序列化的类
	static Card[] BuildSerializableCards() {
		Card[] cardsList = currentPlayerCardManager.cardsArray;
		if (cardsList == null || cardsList.Length != CardsListManager.MAX_TOTAL_CARDS) {
			cardsList = new Card[CardsListManager.MAX_TOTAL_CARDS];
		}
		Card[] serialized = new Card[CardsListManager.MAX_TOTAL_CARDS];
		for (int i = 0; i < CardsListManager.MAX_TOTAL_CARDS; i++) {
			Card card = cardsList[i];
			if (card != null) {
				card.exists = true;
				serialized[i] = card;
			}
			else {
				serialized[i] = new Card { exists = false };
			}
		}
		return serialized;
	}
	/// <summary>
	/// 从本地加载卡组
	/// </summary>
	static void LoadCardsListFromLocal(int setIndex) {
		try {
			string targetPath = cardSetPaths[setIndex];
			if (File.Exists(targetPath)) {
				string json = File.ReadAllText(targetPath);
				CardListWrapper wrapper = JsonUtility.FromJson<CardListWrapper>(json);
				if (wrapper.cards != null) {
					ApplyLoadedCards(wrapper.cards);
				}
				else {
					Debug.LogError($"{Log.perfix}卡组不存在，使用默认卡组");
					currentPlayerCardManager.InitCardsList();
				}
				Debug.Log($"{Log.perfix}从本地加载卡组成功 (卡组编号 {setIndex})");
			}
			else {
				Debug.LogWarning($"{Log.perfix}卡组 {setIndex} 文件不存在，使用预设卡组");
				Card[] presetInstance = PresetCardsList.GetPresetCardsList(setIndex);
				ApplyLoadedCards(presetInstance);
			}
		}
		catch (System.Exception e) {
			Debug.LogError($"{Log.perfix}加载卡组失败: {e.Message}");
			currentPlayerCardManager.InitCardsList();
		}
		CurrentCardSetIndex = setIndex;
		SaveCurrentCardSetIndex(CurrentCardSetIndex);
		currentPlayerCardManager.NotifyCardLevelChanged();
	}
	static void ApplyLoadedCards(Card[] source) {
		Card[] cardsList = currentPlayerCardManager.cardsArray;
		if (cardsList == null || cardsList.Length != CardsListManager.MAX_TOTAL_CARDS) {
			cardsList = new Card[CardsListManager.MAX_TOTAL_CARDS];
		}
		for (int i = 0; i < CardsListManager.MAX_TOTAL_CARDS; i++) {
			Card card = (source != null && i < source.Length) ? source[i] : null;
			if (card == null || !card.exists) {
				cardsList[i] = null;
			}
			else {
				cardsList[i] = card;
				cardsList[i].isSelfCard = true;
				cardsList[i].isVisible = true;
			}
		}
	}






	/// <summary>
	/// 切换卡组
	/// </summary>
	public static void SwitchCardSet(int targetIndex) {
		if (targetIndex < 0 || targetIndex >= cardsGroupPresetCount) {
			Debug.LogWarning($"卡组编号无效: {targetIndex}");
			return;
		}
		SaveCardsListToLocal();
		LoadCardsListFromLocal(targetIndex);
	}
	/// <summary>
	/// 将当前卡组重置为默认
	/// </summary>
	public static void ResetToPresetCards() {
		Card[] defaultCards = PresetCardsList.GetPresetCardsList(CurrentCardSetIndex);
		ApplyLoadedCards(defaultCards);
		currentPlayerCardManager.NotifyCardLevelChanged();
	}
}


/// <summary>
/// 卡组包装类，用于JSON序列化
/// </summary>
[System.Serializable]
public class CardListWrapper {
	public Card[] cards;
}