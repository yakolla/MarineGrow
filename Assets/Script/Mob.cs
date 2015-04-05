using UnityEngine;
using System.Collections;

public class Mob : Creature {

	RefMob				m_refMob;
	bool				m_boss = false;
	MobAI				m_ai;
	GameObject			m_goalForNavigation;

	[SerializeField]
	GameObject			m_prefEffectBlood;
	// Use this for initialization
	new void Start () {
		base.Start();

		m_navAgent.baseOffset = m_refMob.baseCreatureProperty.navMeshBaseOffset;

		if (true == Boss)
		{
			transform.Find("FloatingHealthBarGUI").gameObject.SetActive(true);
		}
	}

	public void Init(RefMob refMob, int mobLevel, Spawn spawn, RefItemSpawn[] refDropItems, bool boss)
	{
		RefMob = refMob;
		Spawn = spawn;
		RefDropItems = refDropItems;
		Boss = boss;
		rigidbody.mass = refMob.mass;

		m_creatureProperty.init(this, m_refMob.baseCreatureProperty, mobLevel);		

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
		case MobAIType.Egg:
			m_ai = new MobAIEgg();
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

	override public void TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		base.TakeDamage(offender, damageDesc);

		if (m_prefEffectBlood != null)
		{
			GameObject effect = (GameObject)Instantiate(m_prefEffectBlood, transform.position, transform.rotation);
			effect.transform.localScale = transform.localScale;
		}

	}

	override public void Death()
	{
		Spawn.OnKillMob(this);

		if (RefMob.eggMob != null)
		{
			Vector3 pos = transform.position;
			pos.y = 0f;
			for(int i = 0; i < RefMob.eggMob.maxCount; ++i)
			{
				Spawn.SpawnMob(RefMob.eggMob.refMob, pos, false, false);
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
