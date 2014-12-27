using UnityEngine;
using System.Collections;

public class Mob : Creature {

	RefMob				m_refMob;

	GameObject			m_goalForNavigation;
	// Use this for initialization
	new void Start () {
		base.Start();

		m_creatureProperty.init(m_refMob.phyDamagePerLevel, m_refMob.phyDefencePerLevel, m_refMob.hpPerLevel);
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();

		if (AutoAttack() == false)
		{
			if (m_goalForNavigation)
			{
				m_navAgent.SetDestination(m_goalForNavigation.transform.position);
				RotateToTarget(m_goalForNavigation.transform.position);
			}
		}
		else
		{
			m_navAgent.Stop();
		}

	}

	public RefMob SpawnDesc
	{
		get {return m_refMob;}
	}
	
	public void SetSpawnDesc(RefMob spawnDesc)
	{
		m_refMob = spawnDesc;
	}

	override public string[] GetAutoTargetTags()
	{
		return new string[]{Creature.Type.Champ.ToString()};
	}

	public void SetTarget(GameObject obj )
	{
		m_goalForNavigation = obj;

	}

	override public void Death()
	{
		GameObject.Find("Dungeon").gameObject.GetComponent<Dungeon>().SpawnItemBox(m_refMob, transform.position);
		
		base.Death();
	}

}
