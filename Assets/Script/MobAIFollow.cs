﻿using UnityEngine;
using System.Collections;

public class MobAIFollow : MobAI {

	Creature m_owner;

	public void SetOwner(Creature owner)
	{
		m_owner = owner;
	}
	// Update is called once per frame
	override public void Update () {

		if (m_mob.AutoAttack() == false)
		{
		}
		
		if (m_owner != null)
			m_navAgent.SetDestination(m_owner.transform.position);
	}


}
