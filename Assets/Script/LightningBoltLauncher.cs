using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningBoltLauncher : Weapon {

	GameObject	m_bullet;


	override public GameObject CreateBullet(float chargingTime)
	{
		if (m_bullet == null)
		{
			m_bullet = base.CreateBullet(chargingTime);
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
