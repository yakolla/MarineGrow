using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningLauncher : Weapon {

	LightningBullet	m_bullet;
	int		m_maxChaining = 1;
	 

	override public void StartFiring(Vector2 targetAngle)
	{		
		if (m_firing == false && isCoolTime() == true )
		{
			if (null == m_bullet)
			{
				m_bullet = CreateBullet(targetAngle, m_gunPoint.transform.position) as LightningBullet;
			}
			m_bullet.Damage = Damage;
			m_bullet.gameObject.SetActive(true);
			m_bullet.MaxChaining = m_maxChaining;

			this.audio.Play();

		}
		if (null != m_bullet)
		{
			Vector3 euler = m_bullet.transform.rotation.eulerAngles;
			euler.y = transform.eulerAngles.y;
			m_bullet.transform.eulerAngles = euler;


		}

		playGunPointEffect();
		m_firing = true;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
		this.audio.Stop();
		if (m_bullet != null)
		{
			m_bullet.StopFiring();
			m_bullet.gameObject.SetActive(false);
		}

		stopGunPointEffect();

	}

	override public void MoreFire()
	{
		base.MoreFire();
		
		m_maxChaining += 1;
	}

	override public void LevelUp()
	{
		base.LevelUp();

		if (m_level % 2 == 0)
		{
			
		}
		else
		{
			m_maxChaining += 1;
		}

	}
}
