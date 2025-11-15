//			using System;
//			using System.IO;
//			using UnityEngine;

//			public class CardsListManager : MonoBehaviour {
//				const int CARD_SET_COUNT = 5;

//				[SerializeField] CardManager cardManager;
//				string cardListDirectory;
//				string[] cardSetPaths;
//				public int CurrentCardSetIndex { get; private set; }

//				public void Init(CardManager targetCardManager) {
//					cardManager = targetCardManager ?? cardManager ?? GetComponent<CardManager>();
//					if (!EnsureCardManager()) {
//						return;
//					}
//					EnsureStorageReady();
//					LoadCardsListFromLocal(0);
//				}
//				public void SaveCardsListToLocal(int setIndex = -1) {
//					if (!EnsureCardManager()) {
//						return;
//					}
//					EnsureStorageReady();
//					setIndex = NormalizeCardSetIndex(setIndex);
//					try {
//						CardListWrapper wrapper = new CardListWrapper {
//							cards = cardManager.BuildSerializableCards(),
//							cardSetIndex = setIndex
//						};
//						string json = JsonUtility.ToJson(wrapper, true);
//						File.WriteAllText(cardSetPaths[setIndex], json);
//						Debug.Log($"卡组 {setIndex} 已保存到: {cardSetPaths[setIndex]}");
//					}
//					catch (Exception e) {
//						Debug.LogError($"保存卡组失败: {e.Message}");
//					}
//				}
//				public void LoadCardsListFromLocal(int setIndex = -1) {
//					if (!EnsureCardManager()) {
//						return;
//					}
//					EnsureStorageReady();
//					setIndex = NormalizeCardSetIndex(setIndex);
//					try {
//						string targetPath = cardSetPaths[setIndex];
//						Card[] cardsToApply = null;
//						if (File.Exists(targetPath)) {
//							string json = File.ReadAllText(targetPath);
//							CardListWrapper wrapper = JsonUtility.FromJson<CardListWrapper>(json);
//							if (wrapper != null && wrapper.cards != null && wrapper.cards.Length == CardManager.MAX_TOTAL_CARDS) {
//								cardsToApply = wrapper.cards;
//							}
//							else {
//								Debug.LogWarning($"{Log.perfix}加载的卡组数据格式不正确，使用默认卡组");
//								cardManager.InitCardsList();
//							}
//						}
//						else {
//							Debug.LogWarning($"{Log.perfix}卡组 {setIndex} 文件不存在，使用预设卡组");
//							cardsToApply = CloneCardArray(PresetCardsList.GetPresetCardsList(setIndex));
//						}
//						if (cardsToApply != null) {
//							cardManager.ApplyLoadedCards(cardsToApply);
//						}
//						else if (cardManager.cardsList == null || cardManager.cardsList.Length == 0) {
//							cardManager.InitCardsList();
//						}
//					}
//					catch (Exception e) {
//						Debug.LogError($"{Log.perfix}加载卡组失败: {e.Message}");
//						cardManager.InitCardsList();
//					}
//					CurrentCardSetIndex = setIndex;
//					cardManager.NotifyCardsListChanged();
//				}
//				public void SwitchCardSet(int targetIndex) {
//					if (!EnsureCardManager()) {
//						return;
//					}
//					if (!IsValidCardSetIndex(targetIndex)) {
//						Debug.LogWarning($"卡组编号无效: {targetIndex}");
//						return;
//					}
//					SaveCardsListToLocal(CurrentCardSetIndex);
//					LoadCardsListFromLocal(targetIndex);
//				}
//				bool EnsureCardManager() {
//					if (cardManager != null) {
//						return true;
//					}
//					cardManager = GetComponent<CardManager>();
//					if (cardManager == null) {
//						Debug.LogError("CardManager组件不存在，无法管理卡组");
//						return false;
//					}
//					return true;
//				}
//				void EnsureStorageReady() {
//					if (string.IsNullOrEmpty(cardListDirectory)) {
//						cardListDirectory = Path.Combine(Application.persistentDataPath, "CardSets");
//					}
//					if (!Directory.Exists(cardListDirectory)) {
//						Directory.CreateDirectory(cardListDirectory);
//					}
//					if (cardSetPaths == null || cardSetPaths.Length != CARD_SET_COUNT) {
//						cardSetPaths = new string[CARD_SET_COUNT];
//						for (int i = 0; i < CARD_SET_COUNT; i++) {
//							cardSetPaths[i] = Path.Combine(cardListDirectory, $"CardList_{i}.json");
//						}
//					}
//				}
//				int NormalizeCardSetIndex(int index) {
//					if (index >= 0 && index < CARD_SET_COUNT) {
//						return index;
//					}
//					return Mathf.Clamp(CurrentCardSetIndex, 0, CARD_SET_COUNT - 1);
//				}
//				bool IsValidCardSetIndex(int index) {
//					return index >= 0 && index < CARD_SET_COUNT;
//				}
//				Card[] CloneCardArray(Card[] source) {
//					if (source == null) {
//						return null;
//					}
//					Card[] result = new Card[source.Length];
//					for (int i = 0; i < source.Length; i++) {
//						result[i] = CloneCard(source[i]);
//					}
//					return result;
//				}
//				Card CloneCard(Card original) {
//					if (original == null) {
//						return null;
//					}
//					return new Card {
//						index = original.index,
//						cardType = original.cardType,
//						level = original.level,
//						hp = original.hp,
//						atk = original.atk,
//						positionX = original.positionX,
//						positionY = original.positionY,
//						exists = original.exists
//					};
//				}
//			}

//			[Serializable]
//			public class CardListWrapper {
//				public Card[] cards;
//				public int cardSetIndex;
//			}