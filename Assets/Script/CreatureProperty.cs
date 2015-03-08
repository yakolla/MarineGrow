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
	float	m_alphaMaxHP = 0f;

	[SerializeField]
	float	m_alphaPhysicalDamage = 0f;

	[SerializeField]
	float	m_alphaCriticalRatio = 0f;

	[SerializeField]
	float	m_alphaCriticalDamage = 0f;

	[SerializeField]
	float	m_alphaPhysicalDefencePoint = 0f;

	[SerializeField]
	float	m_alphaMoveSpeed = 0f;

	float	m_betaMoveSpeed = 1f;

	[SerializeField]
	float	m_alphaLifeSteal = 0f;

	[SerializeField]
	float	m_alphaGainExtraExp = 0f;

	[SerializeField]
	float	m_alphaAttackCoolTime = 0f;


	[SerializeField]
	uint		m_level = 1;

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

	public uint Level
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
		get { return Mathf.FloorToInt(m_level*250*1.1f); }
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
		get {return (m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage + (m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage)*(Level-1)*m_baseProperty.phyDamagePerLevel);}
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
		get {return Mathf.Min(100, m_baseProperty.moveSpeed + AlphaMoveSpeed) * BetaMoveSpeed;}
	}
	
	public float	AlphaMoveSpeed
	{
		get {return m_alphaMoveSpeed;}
		set { m_alphaMoveSpeed = value; }
	}

	public float	BetaMoveSpeed
	{
		get {return m_betaMoveSpeed;}
		set { m_betaMoveSpeed = value; }
	}


	public float LifeSteal
	{
		get{return m_baseProperty.lifeSteal + AlphaLifeSteal;}
	}

	public float AlphaLifeSteal
	{
		get { return m_alphaLifeSteal; }
		set { m_alphaLifeSteal = value; }
	}

	public float	CriticalRatio
	{
		get {return Mathf.Min(100, m_baseProperty.criticalRatio + AlphaCriticalRatio);}
	}

	public float	AlphaCriticalRatio
	{
		get {return m_alphaCriticalRatio;}
		set { m_alphaCriticalRatio = value; }
	}

	public float	CriticalDamage
	{
		get {return m_baseProperty.criticalDamage + AlphaCriticalDamage;}
	}
	
	public float	AlphaCriticalDamage
	{
		get {return m_alphaCriticalDamage;}
		set { m_alphaCriticalDamage = value; }
	}

	public float	GainExtraExp
	{
		get {return AlphaGainExtraExp;}
	}
	
	public float	AlphaGainExtraExp
	{
		get {return m_alphaGainExtraExp;}
		set { m_alphaGainExtraExp = value; }
	}

	public float	AttackCoolTime
	{
		get {return Mathf.Max(m_baseProperty.attackCoolTime + AlphaAttackCoolTime, 0.2f);}
	}
	
	public float	AlphaAttackCoolTime
	{
		get {return m_alphaAttackCoolTime;}
		set { m_alphaAttackCoolTime = value; }
	}

	public void CopyTo(CreatureProperty other)
	{
		other.m_owner = m_owner;
		
		other.m_baseProperty = m_baseProperty;
		other.m_hp = m_hp;
		other.m_alphaMaxHP = m_alphaMaxHP;
		other.m_alphaPhysicalDamage = m_alphaPhysicalDamage;
		other.m_alphaCriticalRatio = m_alphaCriticalRatio;
		other.m_alphaCriticalDamage = m_alphaCriticalDamage;
		other.m_alphaPhysicalDefencePoint = m_alphaPhysicalDefencePoint;
		other.m_alphaMoveSpeed = m_alphaMoveSpeed;
		other.m_betaMoveSpeed = m_betaMoveSpeed;
		other.m_alphaLifeSteal = m_alphaLifeSteal;
		other.m_alphaGainExtraExp = m_alphaGainExtraExp;
		other.m_alphaAttackCoolTime = m_alphaAttackCoolTime;
		other.m_level = m_level;
		other.m_exp = m_exp;
	}
}
