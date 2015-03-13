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
		Vector3 scale = transform.localScale;
		scale *= m_refMob.scale;
		transform.localScale = scale;
	}

	public void Init(RefMob refMob, int mobLevel, Spawn spawn, RefItemSpawn[] refDropItems, bool boss)
	{
		RefMob = refMob;
		Spawn = spawn;
		RefDropItems = refDropItems;
		Boss = boss;
		rigidbody.mass = refMob.mass;

		m_creatureProperty.init(this, m_refMob.baseCreatureProperty);		
		m_creatureProperty.Level = mobLevel;

		GameObject prefDeathEffect = Resources.Load<GameObject>("Pref/mon_skin/"+refMob.prefBody+"_death");
		if (prefDeathEffect != null)
		{
			m_prefDeathEffect = prefDeathEffect;
		}

		foreach(RefMob.WeaponDesc weaponDesc in refMob.refWeaponItems)
		{
			if (weaponDesc.reqLevel > mobLevel)
				continue;

			ItemObject weapon = new ItemObject(new ItemWeaponData(weaponDesc.refItemId, weaponDesc.weaponStat));
			weapon.Item.Evolution = weaponDesc.evolution+(int)(mobLevel * refMob.baseCreatureProperty.evolutionPerLevel);
			weapon.Item.Equip(this);
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
		case MobAIType.Dummy:
			m_ai = new MobAIDummy();
			break;
		case MobAIType.Bomber:
			m_ai = new MobAIBomber();
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

		if (RefMob.eggMob != null)
		{
			for(int i = 0; i < RefMob.eggMob.count; ++i)
			{
				Spawn.spawnMobEgg(RefMob.eggMob.refMob, transform.position, m_creatureProperty.Level);
			}
		}

		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;

		AudioClip sfx = Resources.Load<AudioClip>("SFX/"+RefMob.prefBody+"_death");
		if (sfx != null)
		{
			effect.audio.clip = sfx;
			effect.audio.Play();
		}

		DestroyObject(this.gameObject);
		
		ShakeCamera(0.1f);

	}

}
