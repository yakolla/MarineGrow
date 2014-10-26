using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningBoltLauncher : Weapon {

	GameObject	m_bullet;

	new void Start () {
	
		base.Start();
		m_prefBullet = Resources.Load<GameObject>("Pref/LightningBoltBullet");

	}

	override public GameObject CreateBullet()
	{
		if (m_bullet == null)
		{
			m_bullet = base.CreateBullet();
		}

		return m_bullet;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		if (m_bullet != null)
		{
			Bullet bullet = (Bullet)m_bullet.GetComponent(m_prefBullet.name);
			bullet.StopFiring();
		}
		
	}
}
