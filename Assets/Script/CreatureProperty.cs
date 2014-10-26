using UnityEngine;
using System.Collections;

[System.Serializable]
public class CreatureProperty {

	[SerializeField]
	float	m_hp;

	public float	takeDamage(float damage)
	{
		m_hp -= damage;
		m_hp = Mathf.Max(0, m_hp);

		return m_hp;

	}
}
