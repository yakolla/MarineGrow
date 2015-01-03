using UnityEngine;
using System.Collections;

public class SuicideBombing : Weapon {

	bool m_destroy = false;
	void OnDestroy() {

	}

	override public void StartFiring(Vector2 targetAngle, float chargingTime, FiringDesc[] firingDescs)
	{
		m_firing = true;
	}

	void Update()
	{
		if (m_destroy == true)
		{
			CreateBullet(Vector2.zero, 0f);
			m_creature.Death();
			return;
		}

		if (m_creature.m_targeting != null)
		{
			float d = Vector3.Distance(m_creature.transform.position, transform.position);
			if (d < 2f)
			{
				m_destroy = true;
			}
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		return;
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_creature))
		{
			m_destroy = true;
		}
		
	}
}
