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
		Count
	}

	public enum DebuffType
	{
		Nothing,
		Airbone,
		Count,
	}

	float		m_damage;
	Type		m_type;
	GameObject	m_prefEffect;
	DebuffType	m_debuffType;

	public DamageDesc(float damage, Type type, DebuffType debuffType, GameObject prefEffect)
	{
		m_damage = damage;
		m_type = type;
		m_debuffType = debuffType;
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

	public DebuffType DamageDeBuffType
	{
		get { return m_debuffType;}
	}

	public GameObject	PrefEffect
	{
		get { return m_prefEffect;}
	}
}
