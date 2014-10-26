using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected GameObject	m_aimpoint = null;
	bool					m_firing = false;
	protected	string		m_targetTagName;
	protected	float		m_damage;

	virtual public void Init(GameObject aimpoint, string targetTagName, float damage)
	{
		m_aimpoint = aimpoint;
		m_firing = true;
		m_targetTagName = targetTagName;
		m_damage = damage;
	}

	virtual public void StopFiring()
	{
		m_firing = false;
	}

	public bool IsFiring()
	{
		return m_firing;
	}

}
