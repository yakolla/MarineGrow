using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected GameObject	m_aimpoint = null;
	bool					m_firing = false;
	protected	string		m_targetTagName;

	virtual public void Init(GameObject aimpoint, string targetTagName)
	{
		m_aimpoint = aimpoint;
		m_firing = true;
		m_targetTagName = targetTagName;
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
