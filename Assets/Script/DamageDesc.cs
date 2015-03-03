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
		Count,
	}

	float		m_damage;
	Type		m_type;
	GameObject	m_prefEffect;
	BuffType	m_buffType;

	public DamageDesc(float damage, Type type, BuffType buffType, GameObject prefEffect)
	{
		m_damage = damage;
		m_type = type;
		m_buffType = buffType;
		m_prefEffect = prefEffect;
	}

	public float Damage
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
}
