using UnityEngine;
using System.Collections;

public class SuicideBombing : Weapon {

	bool m_destroy = false;

	public void Init(Creature creature, ItemWeaponData weaponData)
	{
		base.Init(creature, weaponData);

		m_destroy = false;

		creature.EnableNavMeshObstacleAvoidance(false);
	}

	override public void StartFiring(float targetAngle)
	{
		DidStartFiring(0f);
		m_firing = true;
	}
	/*
	void Update()
	{
		if (m_destroy == true)
		{

			return;
		}

		Creature[] targets = Bullet.SearchTarget(transform.position, m_creature.GetMyEnemyType(), AttackRange);
		foreach(Creature target in targets)
		{
			if (target != null)
			{
				CreateBullet(m_firingDescs[0], transform.position);
				m_creature.Death();
				
				m_destroy = true;
				break;
			}
		}
	}
*/
	void OnTriggerEnter(Collider other) {

		if (m_destroy == true)
		{
			return;
		}

		Creature target = other.gameObject.GetComponent<Creature>();
		if (target && Creature.IsEnemy(target, m_creature))
		{
			CreateBullet(m_firingDescs[0], transform.position);
			m_creature.Death();
			
			m_destroy = true;
		}

	}

}
