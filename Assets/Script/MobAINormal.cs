using UnityEngine;
using System.Collections;

public class MobAINormal : MobAI {


	// Update is called once per frame
	override public void Update () {
		if (TimeEffector.Instance.IsStop() == true)
			return;

		if (m_mob.AutoAttack() == false)
		{
			if (m_target)
			{
				m_navAgent.SetDestination(m_target.transform.position);
				m_mob.RotateToTarget(m_target.transform.position);
			}
		}
		else
		{
			m_navAgent.Stop();
		}
	}


}
