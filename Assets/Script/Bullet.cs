using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected GameObject	m_aimpoint = null;
	bool					m_firing = false;
	protected	string		m_targetTagName;
	protected	float		m_chargingTime;
	protected	float		m_damage;
	protected	Creature	m_ownerCreature;

	virtual public void Init(Creature ownerCreature, GameObject aimpoint, float damage, float chargingTime)
	{
		m_aimpoint = aimpoint;
		m_firing = true;
		m_damage = damage;
		m_targetTagName = ownerCreature.TargetTagName;
		m_ownerCreature = ownerCreature;
		m_chargingTime = chargingTime;
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
