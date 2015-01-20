using UnityEngine;
using System.Collections;

public class MobAIDash : MobAI {

	Vector3	m_goal;
	bool	m_breakMode = false;
	float	m_speed;
	GameObject	m_prefAttackGuidedLine;

	override public void Init(Creature mob)
	{
		base.Init(mob);

		m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

		m_prefAttackGuidedLine = Resources.Load<GameObject>("Pref/ef laser point");
	}

	override public void SetTarget(GameObject obj )
	{
		base.SetTarget(obj);

		if (obj != null)
			m_goal = obj.transform.position;

		m_mob.RotateToTarget(m_goal);
		m_navAgent.SetDestination(m_goal);
		m_speed = 0;
		m_breakMode = false;

		m_mob.Spawn.StartCoroutine(EffectAttackGudiedLine(m_mob.transform.position, m_goal, 0));

	}

	IEnumerator EffectAttackGudiedLine(Vector3 start, Vector3 goal, float t)
	{		
		float targetHorAngle = Mathf.Atan2(goal.z-start.z, goal.x-start.x) * Mathf.Rad2Deg;
		GameObject guidedLine = MonoBehaviour.Instantiate (m_prefAttackGuidedLine, start, Quaternion.Euler (0, -targetHorAngle, 0)) as GameObject;
		Vector3 scale = Vector3.one;
		scale.x = Vector3.Distance(start, goal);
		guidedLine.transform.localScale = scale;
		while(t < 1f)
		{
			t += 0.01f;
			yield return null;
		}

		MonoBehaviour.DestroyObject(guidedLine);
	}

	// Update is called once per frame
	override public void Update () {

		if (TimeEffector.Instance.IsStop() == true)
			return;

		m_mob.AutoAttack();
		if (m_target)
		{
			if (m_breakMode == false)
			{
				m_speed += Time.deltaTime*10.5f;
			}
			else
			{
				m_speed -= Time.deltaTime*10.2f;
				if (m_speed <= 0)
				{			
					m_speed = 0f;
					SetTarget(m_target);
				}
			}
			m_navAgent.speed = m_speed;
			m_navAgent.autoBraking = false;
			float d = Vector3.Distance(m_mob.transform.position, m_goal);
			if (d <= 1.1f)
			{
				m_breakMode = true;
			}

		}


	}


}
