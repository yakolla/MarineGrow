using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected GameObject	m_aimpoint = null;
	bool					m_firing = false;
	protected	string		m_targetTagName;


	protected	Creature	m_ownerCreature;

	virtual public void Init(Creature ownerCreature, GameObject aimpoint, string targetTagName, float damage)
	{
		m_aimpoint = aimpoint;
		m_firing = true;
		m_targetTagName = targetTagName;
		m_ownerCreature = ownerCreature;
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
