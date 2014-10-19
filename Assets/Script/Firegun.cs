using UnityEngine;
using System.Collections;

public class FireGun : Weapon {

	GameObject	m_bullet;
	new void Awake () {
		base.Awake();
		m_prefBullet = Resources.Load<GameObject>("Pref/FireGunBullet");
	}

	override public GameObject CreateBullet()
	{
		if (m_firing == false)
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
