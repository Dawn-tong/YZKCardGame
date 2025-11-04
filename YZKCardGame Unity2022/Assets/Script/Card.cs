using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
	Normal,
	Special
}

[System.Serializable]
public class Card
{
	public CardType cardType;

	// 注意：这些字段需要是public用于序列化，但建议通过方法来设置
	public int cardId;  // 卡片在List中的位置ID
	public int level;  // 星级
	public int hp;     // 生命
	public int atk;    // 攻击

	// 默认构造函数
	public Card()
	{
		cardType = CardType.Normal;
		cardId = -1;  // -1表示未分配位置
		level = 1;
		hp = 1;
		atk = 1;
	}

	// 带参数的构造函数
	public Card(CardType type, int level, int hp, int atk)
	{
		this.cardType = type;
		cardId = -1;  // -1表示未分配位置
		SetLevel(level);
		SetHp(hp);
		SetAtk(atk);
	}

	// 带ID的构造函数
	public Card(CardType type, int cardId, int level, int hp, int atk)
	{
		this.cardType = type;
		this.cardId = cardId;
		SetLevel(level);
		SetHp(hp);
		SetAtk(atk);
	}

	/// <summary>
	/// 设置星级 (1~12)，返回是否成功
	/// </summary>
	public bool SetLevel(int newLevel)
	{
		if (newLevel < 1 || newLevel > 12)
		{
			Debug.LogWarning($"星级必须在1~12之间，当前值: {newLevel}");
			return false;
		}

		int oldLevel = level;
		level = newLevel;

		// 调整HP和ATK，使它们之和等于2*星级
		int targetSum = level * 2;
		int currentSum = hp + atk;

		if (currentSum != targetSum)
		{
			// 如果当前和与目标和不一致，按比例调整
			if (currentSum > 0)
			{
				hp = Mathf.Max(1, Mathf.RoundToInt((float)hp / currentSum * targetSum));
				atk = targetSum - hp;
			}
			else
			{
				hp = level;
				atk = level;
			}
		}

		return true;
	}

	/// <summary>
	/// 设置生命值，同时调整攻击力以满足 攻击+生命=2*星级
	/// </summary>
	public bool SetHp(int newHp)
	{
		int targetSum = level * 2;
		int calculatedAtk = targetSum - newHp;

		if (newHp < 1)
		{
			Debug.LogWarning($"生命值必须大于等于1，当前值: {newHp}");
			return false;
		}

		if (calculatedAtk < 1)
		{
			Debug.LogWarning($"设置生命值{newHp}会导致攻击力小于1，无法满足 攻击+生命=2*星级 的约束");
			return false;
		}

		hp = newHp;
		atk = calculatedAtk;
		return true;
	}

	/// <summary>
	/// 设置攻击力，同时调整生命值以满足 攻击+生命=2*星级
	/// </summary>
	public bool SetAtk(int newAtk)
	{
		int targetSum = level * 2;
		int calculatedHp = targetSum - newAtk;

		if (newAtk < 1)
		{
			Debug.LogWarning($"攻击力必须大于等于1，当前值: {newAtk}");
			return false;
		}

		if (calculatedHp < 1)
		{
			Debug.LogWarning($"设置攻击力{newAtk}会导致生命值小于1，无法满足 攻击+生命=2*星级 的约束");
			return false;
		}

		atk = newAtk;
		hp = calculatedHp;
		return true;
	}

	/// <summary>
	/// 获取星级
	/// </summary>
	public int GetLevel()
	{
		return level;
	}

	/// <summary>
	/// 获取生命值
	/// </summary>
	public int GetHp()
	{
		return hp;
	}

	/// <summary>
	/// 获取攻击力
	/// </summary>
	public int GetAtk()
	{
		return atk;
	}

	/// <summary>
	/// 验证卡片数据是否有效
	/// </summary>
	public bool IsValid()
	{
		if (level < 1 || level > 12)
		{
			Debug.LogError($"星级无效: {level}");
			return false;
		}

		if (hp < 1 || atk < 1)
		{
			Debug.LogError($"生命或攻击值无效: HP={hp}, ATK={atk}");
			return false;
		}

		if (hp + atk != level * 2)
		{
			Debug.LogError($"属性总和不符合规则: HP({hp}) + ATK({atk}) = {hp + atk}, 应为 {level * 2}");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 设置卡片ID
	/// </summary>
	public void SetCardId(int id)
	{
		cardId = id;
	}

	/// <summary>
	/// 获取卡片ID
	/// </summary>
	public int GetCardId()
	{
		return cardId;
	}

	/// <summary>
	/// 获取卡片信息字符串
	/// </summary>
	public override string ToString()
	{
		return $"卡牌 [{cardType}] ID:{cardId} 星级:{level} HP:{hp} ATK:{atk}";
	}
}
