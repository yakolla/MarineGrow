﻿using UnityEngine;
using System.Collections;

public class Firegun : Weapon {

	GameObject	m_bullet;

	override public GameObject CreateBullet(Vector2 targetAngle, float chargingTime)
	{
		if (m_bullet == null)
		{
			m_bullet = base.CreateBullet(targetAngle, chargingTime);
		}

		return m_bullet;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		if (m_bullet != null)
		{
			Bullet bullet = (Bullet)m_bullet.GetComponent<Bullet>();
			bullet.StopFiring();
		}

	}
}
