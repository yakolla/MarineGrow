using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningLauncher : Weapon {

	LightningBullet	m_bullet;
	int		m_maxChaining = 1;
	float		m_accSp;

	override public void StartFiring(float targetAngle)
	{		
		if (canConsumeSP() == true )
		{
			if (null == m_bullet)
			{
				m_bullet = CreateBullet(m_firingDescs[0], m_gunPoint.transform.position) as LightningBullet;
			}
			m_bullet.Damage = Damage;
			m_bullet.gameObject.SetActive(true);
			m_bullet.MaxChaining = m_maxChaining;

			if (this.audio.isPlaying == false)
				this.audio.Play();

			m_accSp += SP * Time.deltaTime * coolDownTime();
			if (m_accSp >= 1)
			{
				m_creature.m_creatureProperty.SP -= (int)m_accSp;
				m_accSp -= (int)m_accSp;
			}
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
