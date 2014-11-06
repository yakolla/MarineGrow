using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageDesc {

	public enum Type : int
	{
		Normal,
		Fire,
		Ice,
		Lightinig,
		Count
	}

	float		m_damage;
	Type		m_type;
	GameObject	m_prefEffect;

	public DamageDesc(float damage, Type type, GameObject prefEffect)
	{
		m_damage = damage;
		m_type = type;
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

	public GameObject	PrefEffect
	{
		get { return m_prefEffect;}
	}
}
