using UnityEngine;
using System.Collections;

public class RushLauncher : Weapon {

	Vector3	m_goal;
	float	m_elapsed;
	const float DashSpeed = 9f;
	bool	m_rush = false;

	new void Start()
	{
		base.Start();
		CreateBullet(Vector2.zero, transform.position);
	}


	override public void StartFiring(Vector2 targetAngle)
	{
		if ( isCoolTime() == true )
		{
			m_goal = m_creature.transform.position+m_creature.transform.right.normalized*DashSpeed;
			m_goal.y = m_creature.transform.position.y;
			m_rush = true;
			m_elapsed = 0f;
			m_lastCreated = Time.time;

		}
		m_firing = true;
	}
	
	void Update()
	{
		if (m_rush == false)
			return;

		m_elapsed = m_elapsed+Time.deltaTime;
		m_creature.transform.transform.position = Vector3.Lerp(m_creature.transform.transform.position, m_goal, m_elapsed*0.05f);

		if (m_elapsed >= 1f)
		{
			m_rush = false;
		}
	}
}
