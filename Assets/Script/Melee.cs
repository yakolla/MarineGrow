using UnityEngine;
using System.Collections;

public class Melee : Weapon {
	GameObject[]	m_bullet;
	
	override public void StartFiring(Vector2 targetAngle, float chargingTime, FiringDesc[] firingDescs)
	{		
		if (m_firing == false && isCoolTime() == true )
		{
			m_bullet = new GameObject[firingDescs.Length];
			
			for(int i = 0; i < firingDescs.Length; ++i)
			{
				targetAngle.x = firingDescs[i].angle;
				m_bullet[i] = CreateBullet(targetAngle, chargingTime);
			}
		}
		
		m_firing = true;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
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
