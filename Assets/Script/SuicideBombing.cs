using UnityEngine;
using System.Collections;

public class SuicideBombing : Weapon {

	bool m_destroy = false;
	void OnDestroy() {

	}

	override public void StartFiring(Vector2 targetAngle)
	{
		m_firing = true;
	}

	void Update()
	{
		if (m_destroy == true)
		{

			return;
		}

		if (m_creature.m_targeting != null)
		{
			float d = Vector3.Distance(m_creature.transform.position, m_creature.m_targeting.transform.position);
			if (d < AttackRange)
			{
				CreateBullet(Vector2.zero, transform.position);
				m_creature.Death();

				m_destroy = true;
			}

		}
	}

}
