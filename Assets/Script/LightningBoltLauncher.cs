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
			Bullet bullet = m_bullet.GetComponent<Bullet>();
			bullet.StopFiring();
		}
		
	}
}
