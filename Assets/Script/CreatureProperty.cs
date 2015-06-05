using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureProperty {

	Creature	m_owner;

	[SerializeField]
	RefCreatureBaseProperty	m_baseProperty;
	SecuredType.XInt	m_hp = 0;

	[SerializeField]
	SecuredType.XInt		m_alphaMaxHP = 0;

	[SerializeField]
	SecuredType.XInt		m_alphaPhysicalDamage = 0;

	[SerializeField]
	float	m_alphaCriticalRatio = 0f;

	[SerializeField]
	float	m_alphaCriticalDamage = 0f;

	[SerializeField]
	SecuredType.XInt		m_alphaPhysicalDefencePoint = 0;

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
	float 	m_bulletLength = 1f;

	[SerializeField]
	SecuredType.XInt		m_shield = 0;

	[SerializeField]
	SecuredType.XInt		m_level = 1;

	[SerializeField]
	SecuredType.XInt		m_exp = 0;

	public class WeaponBuffDesc
	{
		public DamageDesc.BuffType	m_buff;
		public float chance;
	}

	[SerializeField]
	WeaponBuffDesc	m_weaponBuffDescs = new WeaponBuffDesc();

	int			m_bombRange = 0;

	public void 	init(Creature owner, RefCreatureBaseProperty baseProperty, int level)
	{
		m_owner = owner;
		m_baseProperty = baseProperty;
		Level = level;
		m_exp = m_baseProperty.exp;
	}

	public float getHPRemainRatio()
	{
		return (float)HP/MaxHP;
	}

	public int MaxHP
	{
		get { return (int)((m_baseProperty.maxHP+AlphaMaxHP)+(m_baseProperty.maxHP+AlphaMaxHP)*(Level-1)*m_baseProperty.hpPerLevel); }
	}

	public int AlphaMaxHP
	{
		get { return m_alphaMaxHP.Value; }
		set { m_alphaMaxHP.Value = value; }
	}

	public int HP
	{
		get { return m_hp.Value; }
	}

	public int Level
	{
		get { return m_level.Value; }
		private set {
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
		get { return Mathf.FloorToInt(m_level.Value*350*1.1f); }
	}

	public int Exp	
	{
		get { return m_exp.Value; }
	}

	public void		giveExp(int exp)
	{
		m_exp.Value += exp;
		while (MaxExp <= m_exp.Value)
		{
			m_exp.Value -= MaxExp;
			++Level;

			if (m_owner != null)
			{
				m_owner.SendMessage("LevelUp");
			}

		}
	}

	public int	givePAttackDamage(int damage)
	{
		m_hp.Value -= damage;
		m_hp.Value = Mathf.Max(0, m_hp.Value);

		return m_hp.Value;
	}

	public int	Heal(int hp)
	{
		m_hp.Value += hp;
		m_hp.Value = Mathf.Min(MaxHP, m_hp.Value);
		
		return m_hp.Value;
	}

	public int	PhysicalAttackDamage
	{
		get {return (int)(m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage + (m_baseProperty.physicalDamage + AlphaPhysicalAttackDamage)*(Level-1)*m_baseProperty.phyDamagePerLevel);}
	}

	public int	AlphaPhysicalAttackDamage
	{
		get {return m_alphaPhysicalDamage.Value;}
		set { m_alphaPhysicalDamage.Value = value; }
	}

	public int	PhysicalDefencePoint
	{
		get {return Mathf.Min(100, (int)(m_baseProperty.physicalDefence + AlphaPhysicalDefencePoint + (m_baseProperty.physicalDefence + AlphaPhysicalDefencePoint)*(Level-1)*m_baseProperty.phyDefencePerLevel));}
	}

	public int	AlphaPhysicalDefencePoint
	{
		get {return m_alphaPhysicalDefencePoint.Value;}
		set { m_alphaPhysicalDefencePoint.Value = value; }
	}

	public float	MoveSpeed
	{
		get {return (m_baseProperty.moveSpeed + AlphaMoveSpeed + Mathf.Min(2, (m_baseProperty.moveSpeed*((Level-1)*m_baseProperty.moveSpeedPerLevel))) * BetaMoveSpeed);}
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

	public float BulletLength
	{
		get {return m_bulletLength;}
		set { m_bulletLength = value; }
	}

	public int Shield
	{
		get {return m_shield.Value;}
		set { m_shield.Value = value; }
	}


	public bool		BackwardOnDamage
	{
		get {return m_baseProperty.backwardOnDamage;}
	}

	public WeaponBuffDesc WeaponBuffDescs
	{
		get {return m_weaponBuffDescs;}
	}

	public DamageDesc.BuffType RandomWeaponBuff
	{
		get {

			if (Random.Range(0f, 1f) < m_weaponBuffDescs.chance)
			{
				return m_weaponBuffDescs.m_buff;
			}

			return DamageDesc.BuffType.Nothing;
		}
	}

	public int SplashRange
	{
		set {m_bombRange = value;}
		get {return m_bombRange;}
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
		other.m_bulletLength = m_bulletLength;

		other.m_weaponBuffDescs.chance = m_weaponBuffDescs.chance;
		other.m_weaponBuffDescs.m_buff = m_weaponBuffDescs.m_buff;
		other.m_shield = m_shield;
		other.m_bombRange = m_bombRange;
	}
}
