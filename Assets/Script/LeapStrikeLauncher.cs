using UnityEngine;
using System.Collections;

public class LeapStrikeLauncher : Weapon {

	Parabola	m_parabola;

	override public void StartFiring(float targetAngle)
	{
		if ( isCoolTime() == true )
		{
			float d = Vector3.Distance(m_creature.transform.position, m_creature.m_targeting.transform.position);

			m_parabola = new Parabola(m_creature.gameObject, d*1.7f, targetAngle*Mathf.Deg2Rad, 70*Mathf.Deg2Rad, 1);
			m_parabola.TimeScale = 0.80f;

			m_creature.CrowdControl(Creature.CrowdControlType.LeapStrike, true);

			StartedFiring(0f);
		}
		m_firing = true;
	}
	
	void Update()
	{
		if (m_parabola != null && false == m_parabola.Update())
		{
			m_creature.CrowdControl(Creature.CrowdControlType.LeapStrike, false);
			CreateBullet(m_firingDescs[0], transform.position);
			m_parabola = null;
		}
	}
}
