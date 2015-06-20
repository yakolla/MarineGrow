using UnityEngine;
using System.Collections;

public class RushLauncher : Weapon {

	Vector3	m_goal;
	float	m_elapsed;
	const float DashSpeed = 9f;
	bool	m_rush = false;
	RushBullet bullet;

	new void Start()
	{
		base.Start();
		bullet = CreateBullet(m_firingDescs[0], transform.position) as RushBullet;
	}


	override public void StartFiring(float targetAngle)
	{
		if ( isCoolTime() == true )
		{
			m_goal = m_creature.transform.position+m_creature.transform.right.normalized*DashSpeed;
			m_goal.y = m_creature.transform.position.y;
			m_rush = true;
			m_elapsed = 0f;
			DidStartFiring(0f);

		}

		bullet.gameObject.SetActive(true);
		m_firing = true;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		bullet.gameObject.SetActive(false);
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
