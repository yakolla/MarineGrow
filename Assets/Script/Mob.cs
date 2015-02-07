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

		if (refMob.dropEggMob != null)
		{
			StartCoroutine(EffectDropEgg(refMob.dropEggMob.refMob, refMob.dropEggMob.count));
		}
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();

		m_ai.Update();

	}

	IEnumerator EffectDropEgg(RefMob eggMob, int maxDrop)
	{

		while(gameObject != null && maxDrop > 0)
		{
			yield return new WaitForSeconds (3f);
			Egg egg = Spawn.spawnMobEgg(eggMob, transform.position, m_creatureProperty.Level);
			egg.audio.Play();

			egg.VRadian[0] = (transform.localEulerAngles.y-180) * Mathf.Deg2Rad;
			egg.VRadian[1] = (transform.localEulerAngles.y-180) * Mathf.Deg2Rad;

			--maxDrop;
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

		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;

		AudioClip sfx = Resources.Load<AudioClip>("SFX/"+RefMob.prefBody+"_death");
		if (sfx != null)
		{
			effect.audio.clip = sfx;
			effect.audio.Play();
		}

		DestroyObject(this.gameObject);
		
		ShakeCamera();

	}

}
