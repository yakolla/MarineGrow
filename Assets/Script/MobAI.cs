using UnityEngine;
using System.Collections;

public class MobAI {

	protected Mob				m_mob;
	protected GameObject 		m_target;
	protected NavMeshAgent		m_navAgent;

	virtual public void	Init(Mob mob)
	{
		m_mob = mob;
		m_navAgent = mob.GetComponent<NavMeshAgent>();
	}

	virtual public void SetTarget(GameObject obj )
	{
		m_target = obj;
	}
	
	// Update is called once per frame
	virtual public void Update () {

	}


}
