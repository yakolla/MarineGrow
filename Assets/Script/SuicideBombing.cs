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
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_creature))
		{
			m_destroy = true;
		}
		
	}
}
