using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected GameObject	m_aimpoint = null;
	bool					m_firing = false;

	virtual public void Init(GameObject aimpoint)
	{
		m_aimpoint = aimpoint;
		m_firing = true;
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
