using UnityEngine;
using System.Collections;

public class Mob : Creature {

	RefMob				m_refMob;
	Dungeon				m_dungeon;
	RefMobSpawn			m_refMobSpawn;
	bool				m_boss = false;

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

	public bool Boss
	{
		get {return m_boss;}
		set {m_boss = value;}
	}

	public RefMob RefMob
	{
		get {return m_refMob;}
		set {m_refMob = value;}
	}
	
	public Dungeon Dungeon
	{
		set {m_dungeon = value;}
	}

	public RefMobSpawn RefMobSpawn
	{
		set {m_refMobSpawn = value;}
		get {return m_refMobSpawn;}
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
		m_dungeon.OnKillMob(this);
		
		base.Death();

	}

}
