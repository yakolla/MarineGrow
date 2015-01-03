using UnityEngine;
using System.Collections;

public class MobAIDash : MobAI {

	Vector3	m_goal;
	bool	m_breakMode = false;
	float	m_speed;

	override public void Init(Mob mob)
	{
		base.Init(mob);

		m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
	}

	override public void SetTarget(GameObject obj )
	{
		base.SetTarget(obj);
		m_goal = obj.transform.position;	
		m_mob.RotateToTarget(m_goal);

		m_speed = 0;
		m_breakMode = false;
	}

	// Update is called once per frame
	override public void Update () {
		if (m_mob.AutoAttack() == false)
		{
			if (m_target)
			{


				if (m_breakMode == false)
				{
					m_speed += Time.deltaTime*0.1f;
				}
				else
				{
					m_speed -= Time.deltaTime*0.1f;
					if (m_speed <= 0)
					{						
						SetTarget(m_target);
					}
				}

				m_mob.transform.Translate(m_speed, 0, 0, m_mob.transform);
				float d = Vector3.Distance(m_mob.transform.position, m_goal);
				if (d < 1f && m_breakMode == false)
				{
					m_breakMode = true;
				}
			}
		}
		else
		{
			m_speed = 0f;
		}

	}


}
