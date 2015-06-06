using UnityEngine;
using System.Collections;

public class Firegun : Weapon {

	FireGunBullet[]	m_bullet;

	override public void StartFiring(float targetAngle)
	{		
		if (m_firing == false && isCoolTime() == true )
		{
			for(int i = 0; i < m_firingDescs.Count; ++i)
			{
				targetAngle = m_firingDescs[i].angle;
				if (m_bullet[i] == null)
				{
					m_bullet[i] = CreateBullet(m_firingDescs[i], m_gunPoint.transform.position) as FireGunBullet;
				}

				m_bullet[i].StartFiring();
				m_bullet[i].gameObject.SetActive(true);
				m_bullet[i].Damage = Damage;

				Vector3 euler = m_bullet[i].transform.rotation.eulerAngles;
				euler.y = transform.eulerAngles.y+targetAngle;
				m_bullet[i].transform.eulerAngles = euler;

				this.audio.Play();
			}

			StartedFiring(0f);
		}

		m_firing = true;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		this.audio.Stop();
		if (m_bullet != null)
		{
			foreach(Bullet bullet in m_bullet)
			{
				if (bullet == null)
					continue;

				bullet.StopFiring();				
				bullet.gameObject.SetActive(false);
			}
		}

	}

	override public void MoreFire()
	{
		base.MoreFire();

		m_bullet = new FireGunBullet[m_firingDescs.Count];
	}

	override public void LevelUp()
	{
		base.LevelUp();

		m_bullet = new FireGunBullet[m_firingDescs.Count];

		if (m_level % 2 == 0)
		{

		}
		else
		{

		}
		
	}
}
