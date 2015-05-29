using UnityEngine;
using System.Collections;

public class MineLauncher : Weapon {

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		GameObject targetObj = m_creature.m_targeting;
		if (targetObj != null)
		{
			startPos = targetObj.transform.position;
		}

		startPos.x += Random.Range(-m_attackRange/2, m_attackRange/2);
		startPos.z += Random.Range(-m_attackRange/2, m_attackRange/2);
		startPos.y = 0;

		return base.CreateBullet(targetAngle, startPos);
	}
}
