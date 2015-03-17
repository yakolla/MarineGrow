using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageDesc {

	public enum Type : int
	{
		Normal,
		Fire,
		Ice,
		Lightining,
		PickUpGold,
		PickUpHealPotion,
		Count
	}

	public enum BuffType
	{
		Nothing,
		Airborne,
		Stun,
		Slow,
		SteamPack,
		Burning,
		Count,
	}

	int			m_damage;
	Type		m_type;
	GameObject	m_prefEffect;
	BuffType	m_buffType;
	bool		m_pushbackOnDamage;

	public DamageDesc(int damage, Type type, BuffType buffType, GameObject prefEffect)
	{
		m_damage = damage;
		m_type = type;
		m_buffType = buffType;
		m_prefEffect = prefEffect;
		m_pushbackOnDamage	= true;
	}

	public int Damage
	{
		get { return m_damage;}
	}

	public Type DamageType
	{
		get { return m_type;}
	}

	public BuffType DamageBuffType
	{
		get { return m_buffType;}
	}

	public GameObject	PrefEffect
	{
		get { return m_prefEffect;}
	}

	public bool	PushbackOnDamage
	{
		get { return m_pushbackOnDamage;}
		set {m_pushbackOnDamage = value;}
	}
}
