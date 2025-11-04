using UnityEngine;
using System.IO;

public class Player : MonoBehaviour {
	public ulong netID;//服务器标记玩家的网络ID
	public int seatID;//用于标记自己在服务器中的座位
	public string playerName;
	public Card[] cardsList;
	public bool isReady = false;
	// 本地保存路径
	private string cardListPath;
	// 普通卡牌指针：记录最后一个普通卡牌的位置（指向空位置的前一个位置）
	private int lastNormalCardIndex;

	// 卡牌位置约束
	private const int MAX_NORMAL_CARDS = 12;    // 最多12张普通卡牌
	private const int NORMAL_CARD_START = 2;    // 普通卡牌起始位置
	private const int MAX_TOTAL_CARDS = 14;     // 最多14张卡牌(2特殊+12普通)

	public void Init() {
		seatID = 0;
		cardsList = new Card[MAX_TOTAL_CARDS];
		cardListPath = Path.Combine(Application.persistentDataPath, "CardList.json");
		InitCardList();
	}
	// 初始化卡牌列表，创建14个位置（前2个位置固定有特殊卡牌）
	private void InitCardList() {
		// 位置0和1：固定创建特殊卡牌
		cardsList[0] = new Card(CardType.Special, 0, 1, 1, 1);
		cardsList[1] = new Card(CardType.Special, 1, 1, 1, 1);
		// 位置2-13：普通卡牌位置（初始为空）
		for (int i = NORMAL_CARD_START; i < MAX_TOTAL_CARDS; i++) {
			cardsList[i] = null;
		}
		// 初始化指针：普通卡牌位置从2开始，指针指向1（最后一个普通卡牌位置的前一个）
		lastNormalCardIndex = NORMAL_CARD_START - 1;
	}
	public Player SetNetID(ulong netID) {
		this.netID = netID;
		return this;
	}
	public Player SetSeatID(int seatID) {
		this.seatID = seatID;
		return this;
	}
	public Player SetPlayerName(string name) {
		playerName = name;
		return this;
	}
	public Player SetReady(bool ready) {
		isReady = ready;
		return this;
	}

	/// <summary>
	/// 添加普通卡片到卡组（自动追加到最后一个位置）
	/// </summary>
	public bool AddNormalCard(Card card) {
		if (card == null) {
			Debug.LogWarning("无法添加空卡片");
			return false;
		}

		// 检查是否超出普通卡牌上限
		int nextPosition = lastNormalCardIndex + 1;
		if (nextPosition >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"普通卡牌已达上限: {MAX_NORMAL_CARDS}");
			return false;
		}
		// 使用指针直接定位到下一个位置
		card.SetCardId(nextPosition);
		cardsList[nextPosition] = card;
		lastNormalCardIndex = nextPosition;  // 更新指针
		Debug.Log($"成功添加普通卡牌到位置 {nextPosition}: {card}");
		return true;
	}

	/// <summary>
	/// 删除指定位置的卡片（通过ID），删除后后面的卡片ID统一-1
	/// </summary>
	public bool DeleteCard(int cardId) {
		if (cardId < 0 || cardId >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"卡片ID无效: {cardId}");
			return false;
		}
		// 特殊卡牌不可删除
		if (cardId < NORMAL_CARD_START) {
			Debug.LogWarning($"无法删除特殊卡牌（位置 {cardId}）");
			return false;
		}
		
		if (cardId > lastNormalCardIndex) {
			Debug.LogWarning($"位置 {cardId} 没有卡片（超出范围）");
			return false;
		}
		// 删除卡片
		cardsList[cardId] = null;
		// 后面的卡片ID统一-1（前移）
		for (int i = cardId + 1; i <= lastNormalCardIndex; i++) {
			if (cardsList[i] != null) {
				cardsList[i].SetCardId(i - 1);
				cardsList[i - 1] = cardsList[i];
				cardsList[i] = null;
			}
		}
		// 更新指针
		lastNormalCardIndex--;
		Debug.Log($"成功删除卡片ID {cardId}，后续卡片已前移");
		return true;
	}

	/// <summary>
	/// 清空卡组
	/// </summary>
	public void ClearCardList()
	{
		InitCardList();
	}

	/// <summary>
	/// 保存卡组到本地
	/// </summary>
	public void SaveCardListToLocal() {
		try {
			// 将数组包装成可序列化的类
			CardListWrapper wrapper = new CardListWrapper { cards = cardsList };
			// 转换为JSON
			string json = JsonUtility.ToJson(wrapper, true);
			// 写入文件
			File.WriteAllText(cardListPath, json);
			Debug.Log($"卡组已保存到: {cardListPath}");
			Debug.Log($"共保存 {cardsList.Length} 张卡片");
		}
		catch (System.Exception e) {
			Debug.LogError($"保存卡组失败: {e.Message}");
		}
	}
	/// <summary>
	/// 从本地加载卡组
	/// </summary>
	public Player LoadCardListFromLocal() {
		try {
			if (File.Exists(cardListPath)) {
				// 读取文件
				string json = File.ReadAllText(cardListPath);
				// 解析JSON
				CardListWrapper wrapper = JsonUtility.FromJson<CardListWrapper>(json);
				// 更新卡组
				if (wrapper.cards != null && wrapper.cards.Length == MAX_TOTAL_CARDS) {
					cardsList = wrapper.cards;
				}
				else {
					Debug.LogWarning("加载的卡组数据格式不正确，使用默认卡组");
					InitCardList();
				}
				// 更新指针位置
				UpdateLastNormalCardIndex();
				Debug.Log($"从本地加载卡组成功: {cardsList.Length} 张卡片");
			}
			else {
				Debug.LogWarning("本地卡组文件不存在");
				InitCardList();
			}
		}
		catch (System.Exception e) {
			Debug.LogError($"加载卡组失败: {e.Message}");
			InitCardList();
		}
		
		return this;
	}
	/// <summary>
	/// 更新最后一个普通卡牌的位置指针
	/// </summary>
	private void UpdateLastNormalCardIndex() {
		// 从后往前查找最后一个普通卡牌的位置
		for (int i = MAX_TOTAL_CARDS - 1; i >= NORMAL_CARD_START; i--) {
			if (cardsList[i] != null) {
				lastNormalCardIndex = i;
				return;
			}
		}
		
		// 如果没有找到普通卡牌，指针指向起始位置前一个
		lastNormalCardIndex = NORMAL_CARD_START - 1;
	}
	/// <summary>
	/// 从网络加载卡组
	/// </summary>
	public Player LoadCardListFromNet(Card[] cardsList) {
		this.cardsList = cardsList;
		return this;
	}

	// /// <summary>
	// /// 获取普通卡牌数量
	// /// </summary>
	// public int GetNormalCardCount()
	// {
	// 	// 指针值减去起始位置，再+1就是卡牌数量
	// 	int count = lastNormalCardIndex - NORMAL_CARD_START + 1;
	// 	return count > 0 ? count : 0;  // 确保不返回负数
	// }

	/// <summary>
	/// 获取指定位置的卡片
	/// </summary>
	public Card GetCard(int position) {
		if (position < 0 || position >= MAX_TOTAL_CARDS) {
			Debug.LogWarning($"位置无效: {position}");
			return null;
		}
		return cardsList[position];
	}

	/// <summary>
	/// 验证卡组是否有效(规则不同就无法加入游戏)
	/// </summary>
	public bool IsCardListValid() {
		//验证卡牌有效性
		if(cardsList.Length <=0 || cardsList.Length > MAX_TOTAL_CARDS)
		{
			Debug.LogError($"卡牌数量无效: {cardsList.Length}/{MAX_TOTAL_CARDS}");
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
}
