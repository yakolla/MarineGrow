using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningBoltLauncher : Weapon {

	GameObject	m_bullet;


	override public GameObject CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		if (m_bullet == null)
		{
			m_bullet = base.CreateBullet(targetAngle, startPos);
		}
		else{
			m_callbackCreateBullet();
		}
		return m_bullet;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		this.audio.Stop();
		if (m_bullet != null)
		{
			Bullet bullet = m_bullet.GetComponent<Bullet>();
			bullet.StopFiring();
		}
		
	}
}
