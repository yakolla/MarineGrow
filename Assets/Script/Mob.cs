using UnityEngine;
using System.Collections;

public class Mob : Creature {

	RefMob				m_refMob;
	bool				m_boss = false;
	MobAI				m_ai;
	GameObject			m_goalForNavigation;
	// Use this for initialization
	new void Start () {
		base.Start();

		m_navAgent.baseOffset = m_refMob.baseCreatureProperty.navMeshBaseOffset;
	}

	public void Init(RefMob refMob, Spawn spawn, RefItemSpawn[] refDropItems, bool boss)
	{
		RefMob = refMob;
		Spawn = spawn;
		RefDropItems = refDropItems;
		Boss = boss;

		m_creatureProperty.init(this, m_refMob.baseCreatureProperty);
		GameObject prefDeathEffect = Resources.Load<GameObject>("Pref/mon_skin/"+refMob.prefBody+"_death");
		if (prefDeathEffect != null)
		{
			m_prefDeathEffect = prefDeathEffect;
		}


		switch(refMob.mobAI)
		{
		case MobAIType.Normal:
			m_ai = new MobAINormal();
			break;
		case MobAIType.Rotation:
			m_ai = new MobAIRotation();
			break;
		case MobAIType.Dash:
			m_ai = new MobAIDash();
			break;
		case MobAIType.Revolution:
			m_ai = new MobAIRevolution();
			break;
		case MobAIType.ItemShuttle:
			m_ai = new MobAIItemShuttle();
			break;
		}

		m_ai.Init(this);
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();

		m_ai.Update();

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

	override public string[] GetAutoTargetTags()
	{
		return new string[]{Creature.Type.Champ.ToString()};
	}

	public void SetTarget(GameObject obj )
	{
		m_ai.SetTarget(obj);

	}

	override public void Death()
	{
		Spawn.OnKillMob(this);
		
		base.Death();

	}

}
