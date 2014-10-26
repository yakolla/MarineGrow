using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreatureProperty {

	[SerializeField]
	float	m_maxHP = 0;
	float 	m_hp;

	[SerializeField]
	float	m_damage = 0;
	float	m_alphaDamage = 0;

	public void 	init()
	{
		m_hp = m_maxHP;
	}

	public float getHPRemainRatio()
	{
		return m_hp/m_maxHP;
	}

	public float MaxHP
	{
		get { return m_maxHP; }
	}

	public float HP
	{
		get { return m_hp; }
	}

	public float	takeDamage(float damage)
	{
		m_hp -= damage;
		m_hp = Mathf.Max(0, m_hp);

		return m_hp;
	}

	public void	offsetDamage(float damage)
	{
		m_alphaDamage += damage;
	}

	public float	getDamage()
	{
		return m_damage+m_alphaDamage;
	}
}
