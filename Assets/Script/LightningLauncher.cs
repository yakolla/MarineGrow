using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningLauncher : Weapon {

	GameObject[]	m_bullet;
	
	override public void StartFiring(Vector2 targetAngle)
	{		
		if (m_firing == false && isCoolTime() == true )
		{
			m_bullet = new GameObject[m_firingDescs.Count];
			
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
			foreach(GameObject obj in m_bullet)
			{
				Bullet bullet = (Bullet)obj.GetComponent<Bullet>();
				bullet.StopFiring();
			}
			
			m_bullet = null;
		}
		
	}
}
