using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreatureProperty {

	[SerializeField]
	float	m_baseMaxHP = 0;
	float 	m_hp;

	[SerializeField]
	float	m_pATKDamage = 0;

	[SerializeField]
	[Range (0, 100)]
	float	m_pDefencePoint = 0;

	[SerializeField]
	int		m_level = 1;

	[SerializeField]
	int		m_exp = 0;

	public void 	init()
	{
		m_hp = m_baseMaxHP;
	}

	public float getHPRemainRatio()
	{
		return HP/MaxHP;
	}

	public float MaxHP
	{
		get { return m_baseMaxHP*Level; }
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

	public void		giveExp(int exp)
	{
		m_exp += exp;
		while (MaxExp <= m_exp)
		{
			m_exp -= MaxExp;
			++m_level;
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

	public float	PAttackDamage
	{
		get {return m_pATKDamage*Level;}
	}

	public float	PDefencePoint
	{
		get {return Mathf.Min(100, m_pDefencePoint);}
	}
}
