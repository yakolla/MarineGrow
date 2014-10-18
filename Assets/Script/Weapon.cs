using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	GameObject		m_aimpoint;
	protected GameObject		m_prefBullet;
	protected	bool	m_firing = false;
	public float	m_coolTime = 0.5f;
	float			m_lastCreated = 0;

	protected void Awake () {

		m_aimpoint = this.transform.Find("Aimpoint").gameObject;
	}

	virtual public void CreateBullet()
	{
		Vector3 pos = m_aimpoint.transform.position;
		GameObject obj = Instantiate (m_prefBullet, pos, transform.rotation) as GameObject;
		Bullet bullet = (Bullet)obj.GetComponent(m_prefBullet.name);
		bullet.Init(m_aimpoint);

		m_lastCreated = Time.time;
	}

	public void StartFiring()
	{
		if (m_lastCreated + m_coolTime < Time.time )
		{
			CreateBullet();
		}
		m_firing = true;
	}

	public void StopFiring()
	{
		m_firing = false;
	}


	void Update()
	{
		if (m_firing == true)
		{
			if (m_lastCreated + m_coolTime < Time.time )
			{
				CreateBullet();
			}
		}

	}

}
