using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningBoltLauncher : Weapon {

	GameObject	m_bullet;


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
			Bullet bullet = m_bullet.GetComponent<Bullet>();
			bullet.StopFiring();
		}
		
	}
}
