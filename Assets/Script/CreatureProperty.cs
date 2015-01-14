using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureProperty {

	Creature	m_owner;

	[SerializeField]
	RefCreatureBaseProperty	m_baseProperty;
	float 	m_hp;

	[SerializeField]
	float	m_alphaMaxHP = 0;

	[SerializeField]
	float	m_alphaPhysicalDamage = 0;

	[SerializeField]
	float	m_alphaPhysicalDefencePoint = 0;

	[SerializeField]
	float	m_alphaMoveSpeed = 0f;

	[SerializeField]
	int		m_level = 1;

	[SerializeField]
	int		m_exp = 0;

	public void 	init(Creature owner, RefCreatureBaseProperty baseProperty)
	{
		m_owner = owner;
		m_baseProperty = baseProperty;
		m_exp = m_baseProperty.exp;
	}

	public float getHPRemainRatio()
	{
		return HP/MaxHP;
	}

	public float MaxHP
	{
		get { return ((m_baseProperty.maxHP+AlphaMaxHP)+(m_baseProperty.maxHP+AlphaMaxHP)*(Level-1)*m_baseProperty.hpPerLevel)*100; }
	}

	public float AlphaMaxHP
	{
		get { return m_alphaMaxHP; }
		set { m_alphaMaxHP = value; }
	}

	public float HP
	{
		get { return m_hp; }
	}

	public int Level
	{
		get { return m_level; }
		set {
			m_level = value;
			m_hp = MaxHP;
		}
	}

	public float getExpRemainRatio()
	{
		return (float)Exp/MaxExp;
	}

	public int MaxExp
	{
		get { return Mathf.FloorToInt(m_level*350*1.1f); }
	}

	public int Exp	
	{
		get { return m_exp; }
	}

	public void		giveExp(int exp)
	{
		m_exp += exp;
		while (MaxExp <= m_exp)
		{
			m_exp -= MaxExp;
			++Level;

			if (m_owner != null)
			{
				m_owner.SendMessage("LevelUp");
			}

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
		get {return m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage + (m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage)*(Level-1)*m_baseProperty.phyDamagePerLevel;}
	}

	public float	AlphaPhysicalAttackDamage
	{
		get {return m_alphaPhysicalDamage;}
		set { m_alphaPhysicalDamage = value; }
	}

	public float	PhysicalDefencePoint
	{
		get {return Mathf.Min(100, m_baseProperty.physicalDefence + AlphaPhysicalDefencePoint + (m_baseProperty.physicalDefence + AlphaPhysicalDefencePoint)*(Level-1)*m_baseProperty.phyDefencePerLevel);}
	}

	public float	AlphaPhysicalDefencePoint
	{
		get {return m_alphaPhysicalDefencePoint;}
		set { m_alphaPhysicalDefencePoint = value; }
	}

	public float	MoveSpeed
	{
		get {return Mathf.Min(100, m_baseProperty.moveSpeed + AlphaMoveSpeed);}
	}
	
	public float	AlphaMoveSpeed
	{
		get {return m_alphaMoveSpeed;}
		set { m_alphaMoveSpeed = value; }
	}
}
