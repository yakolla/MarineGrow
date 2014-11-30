using UnityEngine;
using System.Collections;

public class Boss : Enemy {

	// Update is called once per frame
	new void Update () {

		AutoAttack();
		
	}

	override protected bool AutoAttack() {
		if (m_targeting != null)
		{
			if (false == inAttackRange(m_targeting))
			{
				m_targeting = null;
			}
		}
		
		if (m_targeting == null)
		{
			m_targeting = searchTarget();
		}
		
		if (m_targeting != null)
		{
			Vector2 dir = RotateToTarget(m_targeting.transform.position);
			m_weaponHolder.GetWeapon().StartFiring(dir, 0, m_firingDescs);

			return true;
		}
		
		m_targeting = null;
		m_weaponHolder.GetWeapon().StopFiring();
		return false;
	}
}
