using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureProperty {

	[SerializeField]
	float	m_baseMaxHP = 0;
	float 	m_hp;

	[SerializeField]
	float	m_basePhysicalDamage = 0;

	[SerializeField]
	[Range (0, 100)]
	float	m_basePhysicalDefence = 0;

	[SerializeField]
	int		m_level = 1;

	[SerializeField]
	int		m_exp = 0;

	[SerializeField]
	int		m_gold = 0;

	public delegate void CallbackOnLevelUp();
	public CallbackOnLevelUp	m_callbackLevelup = delegate(){};

	public void 	init()
	{
		m_hp = MaxHP;
	}

	public float getHPRemainRatio()
	{
		return HP/MaxHP;
	}

	public float MaxHP
	{
		get { return m_baseMaxHP*100; }
	}

	public float BaseMaxHP
	{
		get { return m_baseMaxHP; }
		set { m_baseMaxHP = value; }
	}

	public float HP
	{
		get { return m_hp; }
	}

	public int Level
	{
		get { return m_level; }
	}

	public float getExpRemainRatio()
	{
		return (float)Exp/MaxExp;
	}

	public int MaxExp
	{
		get { return Mathf.FloorToInt(m_level*100*1.1f); }
	}

	public int Exp	
	{
		get { return m_exp; }
	}

	public int Gold
	{
		get { return m_gold; }
		set { m_gold = value; }
	}

	public void		giveExp(int exp)
	{
		m_exp += exp;
		while (MaxExp <= m_exp)
		{
			m_exp -= MaxExp;
			++m_level;
			m_callbackLevelup();
			m_hp = MaxHP;
		}
	}

	public float	givePAttackDamage(float damage)
	{
		m_hp -= damage;
		m_hp = Mathf.Max(0, m_hp);

		return m_hp;
	}

	public float	Heal(int hp)
	{
		m_hp += hp;
		m_hp = Mathf.Min(MaxHP, m_hp);
		
		return m_hp;
	}

	public float	PhysicalAttackDamage
	{
		get {return m_basePhysicalDamage;}
		set { m_basePhysicalDamage = value; }
	}

	public float	PhysicalDefencePoint
	{
		get {return Mathf.Min(100, m_basePhysicalDefence);}
		set { m_basePhysicalDefence = value; }
	}
}
