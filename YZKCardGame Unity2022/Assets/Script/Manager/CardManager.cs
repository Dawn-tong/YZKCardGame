using ProtoMessage;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardManager : MonoBehaviour {
	// 卡牌位置约束
	const int NORMAL_CARD_START = 2;    // 普通卡牌起始位置
	public const int MAX_TOTAL_CARDS = 14;     // 最多14张卡牌(2特殊+12普通)
	const int CARD_SET_COUNT = 5;       // 支持的卡组数量
	[HideInInspector] public int cardMaxLevel = 10;		// 卡牌最高10星
	[HideInInspector] public int maxTotalLevel = 55;      // 所有卡牌最多55星

	public Player owner;
	public Card[] cardsList;
	public event Action CardLevelChanged;
	public int CurrentCardSetIndex { get; private set; } = 0;

	//初始化CardManager
	public void Init(Player owner) {
		this.owner = owner;
		cardsList = new Card[MAX_TOTAL_CARDS];
		InitializeCardSetStorage();
		LoadCardsListFromLocal(CurrentCardSetIndex);
	}
	





	Card GetCard(int index) {
		if (index < 0 || index >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"下标无效: {index}");
			return null;
		}
		return cardsList[index];
	}
	/// <summary>
	/// 获取卡牌总星级
	/// </summary>
	public int CalSumOfCardsLevel() {
		int sum = 0;
		for (int i = NORMAL_CARD_START; i < MAX_TOTAL_CARDS; i++) {
			if (cardsList[i] != null) {
				sum += cardsList[i].level;
			}
		}
		return sum;
	}
	public bool AddNormalCard(int cardIndex) {
		if(cardIndex < 0 || cardIndex >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"卡片ID无效: {cardIndex}");
			return false;
		}
		if(cardsList[cardIndex] != null) {
			Debug.LogWarning($"位置 {cardIndex} 已有卡片");
			return false;
		}
		if(CalSumOfCardsLevel() >= maxTotalLevel) {
			Debug.LogWarning($"总等级超出限制: {maxTotalLevel}");
			return false;
		}
		cardsList[cardIndex] = new Card() { exists = true, index = cardIndex, cardType = CardType.Normal, level = 1, hp = 1, atk = 1};
		CardLevelChanged?.Invoke();
		return true;
	}
	/// <summary>
	/// 重写指定位置的卡片
	/// </summary>
	public bool OverwriteCard(int cardIndex, Card card) {
		if (cardIndex < 0 || cardIndex >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"卡片ID无效: {cardIndex}");
			return false;
		}
		cardsList[cardIndex] = card;
		CardLevelChanged?.Invoke();
		return true;
	}
	public bool OverwriteCard(int cardIndex, CardInfo cardInfo) {
		Card card = new Card();
		card.exists = true;
		card.index = cardInfo.index;
		card.cardType = (CardType)cardInfo.cardType;
		card.level = cardInfo.level;
		card.hp = cardInfo.hp;
		card.atk = cardInfo.atk;
		card.positionX = cardInfo.positionX;
		card.positionY = cardInfo.positionY;
		Debug.Log(card.ToString());
		return OverwriteCard(cardIndex, card);
	}
	/// <summary>
	/// 删除指定位置的卡片
	/// </summary>
	public bool DeleteCard(int cardIndex) {
		if (cardIndex < 0 || cardIndex >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"卡片ID无效: {cardIndex}");
			return false;
		}
		// 特殊卡牌不可删除
		if (cardIndex < NORMAL_CARD_START) {
			Debug.LogWarning($"无法删除特殊卡牌（位置 {cardIndex}）");
			return false;
		}
		
		if (cardsList[cardIndex] == null) {
			Debug.LogWarning($"位置 {cardIndex} 没有卡片");
			return false;
		}
		// 删除卡片
		cardsList[cardIndex] = null;
		CardLevelChanged?.Invoke();
		Debug.Log($"成功删除卡片(ID={cardIndex})");
		return true;
	}
	public bool CardLevelUp(int index) {
		Card card = GetCard(index);
		if (card == null || card.level >= cardMaxLevel) {
			return false;
		}
		if (CalSumOfCardsLevel() >= maxTotalLevel) {
			return false;
		}
		card.level++;
		card.hp++;
		card.atk++;
		CardLevelChanged?.Invoke();
		return true;
	}
	public bool CardLevelDown(int index) {
		Card card = GetCard(index);
		if(card == null || card.level <= 1) {
			return false;
		}
		card.level--;
		//降低生命值
		if(card.hp <= 1) {
			//生命值不足时降低攻击力
			card.atk--;
		}
		else {
			card.hp--;
		}
		//降低攻击力
		if(card.atk <= 1) {
			//攻击力不足时降低生命值
			card.hp--;
		}
		else {
			card.atk--;
		}
		CardLevelChanged?.Invoke();
		return true;
	}
	public bool CardHpUp(int index) {
		Card card = GetCard(index);
		if(card == null || card.atk <= 1) {
			return false;
		}
		card.hp++;
		card.atk--;
		return true;
	}
	public bool CardAtkUp(int index) {
		Card card = GetCard(index);
		if(card == null || card.hp <= 1) {
			return false;
		}
		card.atk++;
		card.hp--;
		return true;
	}






	//交换委托
	public delegate void CardSwapDelegate();
	public event CardSwapDelegate OnCardSwap;
	//寻找卡片
	public Card FindCardByPosition(int positionX, int positionY) {
		for (int i = 0; i < MAX_TOTAL_CARDS; i++) {
			if(cardsList[i] != null && cardsList[i].positionX == positionX && cardsList[i].positionY == positionY) {
				return cardsList[i];
			}
		}
		return null;
	}
	//放置卡牌
	public void PutCardToPosition(Card card, int positionX, int positionY) {
		Card oldCard = FindCardByPosition(positionX, positionY);
		if(oldCard == null) {
			card.positionX = positionX;
			card.positionY = positionY;
		}
		else{
			(card.positionX, oldCard.positionX) = (oldCard.positionX, card.positionX);
			(card.positionY, oldCard.positionY) = (oldCard.positionY, card.positionY);
		}
		OnCardSwap?.Invoke();
	}






	string cardListDirectory;    // 卡组保存目录
	string[] cardSetPaths;       // 各卡组文件路径
	//初始化卡组存储
	void InitializeCardSetStorage() {
		cardListDirectory = Path.Combine(Application.persistentDataPath, "CardSets");
		if (!Directory.Exists(cardListDirectory)) {
			Directory.CreateDirectory(cardListDirectory);
		}
		if (cardSetPaths == null || cardSetPaths.Length != CARD_SET_COUNT) {
			cardSetPaths = new string[CARD_SET_COUNT];
		}
		for (int i = 0; i < CARD_SET_COUNT; i++) {
			cardSetPaths[i] = Path.Combine(cardListDirectory, $"CardList_{i}.json");
		}
	}
	/// <summary>
	/// 保存卡组到本地
	/// </summary>
	public void SaveCardsListToLocal(int setIndex = -1) {
		if (setIndex < 0 || setIndex >= CARD_SET_COUNT) {
			setIndex = CurrentCardSetIndex;
		}
		try {
			CardListWrapper wrapper = new CardListWrapper {
				cards = BuildSerializableCards(),
				cardSetIndex = setIndex
			};
			string json = JsonUtility.ToJson(wrapper, true);
			string targetPath = cardSetPaths[setIndex];
			File.WriteAllText(targetPath, json);
			Debug.Log($"卡组 {setIndex} 已保存到: {targetPath}");
		}
		catch (System.Exception e) {
			Debug.LogError($"保存卡组失败: {e.Message}");
		}
	}
	// 将数组包装成可序列化的类
	Card[] BuildSerializableCards() {
		if (cardsList == null || cardsList.Length != MAX_TOTAL_CARDS) {
			cardsList = new Card[MAX_TOTAL_CARDS];
		}
		Card[] serialized = new Card[MAX_TOTAL_CARDS];
		for (int i = 0; i < MAX_TOTAL_CARDS; i++) {
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
	public void LoadCardsListFromLocal(int setIndex = -1) {
		if (setIndex < 0 || setIndex >= CARD_SET_COUNT) {
			setIndex = CurrentCardSetIndex;
		}
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
					InitCardsList();
				}
				Debug.Log($"{Log.perfix}从本地加载卡组成功: {cardsList.Length} 张卡片 (卡组编号 {setIndex})");
			}
			else {
				Debug.LogWarning($"{Log.perfix}卡组 {setIndex} 文件不存在，使用预设卡组");
				Card[] presetInstance = PresetCardsList.GetPresetCardsList(setIndex);
				ApplyLoadedCards(presetInstance);
			}
		}
		catch (System.Exception e) {
			Debug.LogError($"{Log.perfix}加载卡组失败: {e.Message}");
			InitCardsList();
		}
		CurrentCardSetIndex = setIndex;
		CardLevelChanged?.Invoke();
	}
	void ApplyLoadedCards(Card[] source) {
		if (cardsList == null || cardsList.Length != MAX_TOTAL_CARDS) {
			cardsList = new Card[MAX_TOTAL_CARDS];
		}
		for (int i = 0; i < MAX_TOTAL_CARDS; i++) {
			Card card = (source != null && i < source.Length) ? source[i] : null;
			if (card == null || !card.exists) {
				cardsList[i] = null;
			}
			else {
				cardsList[i] = card;
			}
		}
	}
	/// <summary>
	/// 切换卡组
	/// </summary>
	public void SwitchCardSet(int targetIndex) {
		if (targetIndex < 0 || targetIndex >= CARD_SET_COUNT) {
			Debug.LogWarning($"卡组编号无效: {targetIndex}");
			return;
		}
		SaveCardsListToLocal(CurrentCardSetIndex);
		LoadCardsListFromLocal(targetIndex);
	}




	

	// 初始化为默认卡牌列表，创建14个位置（前2个位置固定有特殊卡牌）
	public void InitCardsList() {
		cardsList[0] = new Card() { exists = true, index = 0, cardType = CardType.Bomb };
		cardsList[1] = new Card() { exists = true, index = 1, cardType = CardType.Bomb };
		for (int i = 1; i <= 10; i++) {
			cardsList[i + 1] = new Card() {
				exists = true,
				index = i + 1,
				cardType = CardType.Normal,
				level = i,
				hp = i,
				atk = i,
			};
		}
		cardsList[12] = null;
		cardsList[13] = null;
	}
	/// <summary>
	/// 从网络加载卡组
	/// </summary>
	public void LoadCardsListFromNet(List<CardInfo> cardInfos) {
		ClearCards();
		foreach (CardInfo cardInfo in cardInfos) {
			OverwriteCard(cardInfo.index, cardInfo);
		}
	}
	//将卡组转换为proto消息
	public void ConvertCardsListToProtoMessage(List<CardInfo> cardInfos) {
		foreach(Card card in cardsList) {
			if(card == null) continue;
			CardInfo cardInfo = new CardInfo();
			cardInfo.index = card.index;
			cardInfo.cardType = (int)card.cardType;
			cardInfo.level = card.level;
			cardInfo.hp = card.hp;
			cardInfo.atk = card.atk;
			cardInfo.positionX = card.positionX;
			cardInfo.positionY = card.positionY;
			cardInfos.Add(cardInfo);
		}
	}
	//清空cardsList
	public void ClearCards() {
		for (int i = 0; i < MAX_TOTAL_CARDS; i++) {
			cardsList[i] = null;
		}
	}






	/// <summary>
	/// 验证卡组是否有效(规则不同就无法加入游戏)
	/// </summary>
	public bool IsValidCardsList() {
		//验证卡牌有效性
		if(cardsList == null || cardsList.Length <= 0 || cardsList.Length > MAX_TOTAL_CARDS) {
			Debug.LogError($"卡牌数量无效: {(cardsList == null ? 0 : cardsList.Length)}/{MAX_TOTAL_CARDS}");
			return false;
		}
		return true;
	}
}

/// <summary>
/// 卡组包装类，用于JSON序列化
/// </summary>
[System.Serializable]
public class CardListWrapper {
	public Card[] cards;
	public int cardSetIndex;
}
