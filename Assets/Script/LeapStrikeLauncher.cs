using UnityEngine;
using System.Collections;

public class LeapStrikeLauncher : Weapon {

	Parabola	m_parabola;

	override public void StartFiring(Vector2 targetAngle)
	{
		if ( isCoolTime() == true )
		{
			float d = Vector3.Distance(m_creature.transform.position, m_creature.m_targeting.transform.position);

			m_parabola = new Parabola(m_creature.gameObject, d, Random.Range(5f, 7f), targetAngle.x*Mathf.Deg2Rad, Random.Range(1.3f, 1.57f), 1);
			m_parabola.TimeScale = 0.80f;

			m_creature.CrowdControl(Creature.CrowdControlType.LeapStrike, true);
			m_lastCreated = Time.time+m_coolTime;
		}
		m_firing = true;
	}
	
	void Update()
	{
		if (m_parabola != null && false == m_parabola.Update())
		{
			m_creature.CrowdControl(Creature.CrowdControlType.LeapStrike, false);
			CreateBullet(Vector2.zero, transform.position);
			m_parabola = null;
		}
	}
}
