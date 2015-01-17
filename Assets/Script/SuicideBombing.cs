using UnityEngine;
using System.Collections;

public class SuicideBombing : Weapon {

	bool m_destroy = false;
	void OnDestroy() {

	}

	override public void StartFiring(Vector2 targetAngle, FiringDesc[] firingDescs)
	{
		m_firing = true;
	}

	void Update()
	{
		if (m_destroy == true)
		{
			CreateBullet(Vector2.zero, m_gunPoint.transform.position);
			m_creature.Death();
			return;
		}

		if (m_creature.m_targeting != null)
		{
			float d = Vector3.Distance(m_creature.transform.position, transform.position);
			if (d < AttackRange)
			{
				m_destroy = true;
			}
		}
	}

}
