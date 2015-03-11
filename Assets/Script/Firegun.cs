using UnityEngine;
using System.Collections;

public class Firegun : Weapon {

	Bullet[]	m_bullet;

	override public void StartFiring(Vector2 targetAngle)
	{		
		if (m_firing == false && isCoolTime() == true )
		{
			m_bullet = new Bullet[m_firingDescs.Count];

			for(int i = 0; i < m_firingDescs.Count; ++i)
			{
				targetAngle.x = m_firingDescs[i].angle;
				m_bullet[i] = CreateBullet(targetAngle, m_gunPoint.transform.position);
			}
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
				bullet.StopFiring();
			}

			m_bullet = null;
		}

	}
}
