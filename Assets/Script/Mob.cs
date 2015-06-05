using UnityEngine;
using System.Collections;

public class Mob : Creature {

	RefMob				m_refMob;
	bool				m_boss = false;
	MobAI				m_ai;

	[SerializeField]
	GameObject			m_prefEffectBlood;

	public void Init(RefMob refMob, int mobLevel, RefItemSpawn[] refDropItems, bool boss)
	{
		base.Init();

		RefMob = refMob;
		RefDropItems = refDropItems;
		Boss = boss;
		rigidbody.mass = refMob.mass;

		m_creatureProperty.init(this, m_refMob.baseCreatureProperty, mobLevel);

		if (Random.Range(0f, 1f) < 0.3f)
			m_creatureProperty.BetaMoveSpeed = 1.7f;


		m_navAgent.baseOffset = m_refMob.baseCreatureProperty.navMeshBaseOffset;
		
		if (true == Boss)
		{
			transform.Find("FloatingHealthBarGUI").gameObject.SetActive(true);
		}

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
		if (m_behaviourType == BehaviourType.Death)
			return;

		m_behaviourType = BehaviourType.Death;

		Const.GetSpawn().OnKillMob(this);

		if (RefMob.eggMob != null)
		{
			Vector3 pos = transform.position;
			pos.y = 0f;
			for(int i = 0; i < RefMob.eggMob.maxCount; ++i)
			{
				Const.GetSpawn().SpawnMob(RefMob.eggMob.refMob, pos, false, false);
			}
		}

		GameObject effect = (GameObject)GameObject.Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;

		AudioClip sfx = Resources.Load<AudioClip>("SFX/"+RefMob.prefBody+"_death");
		if (sfx != null)
		{
			effect.audio.clip = sfx;
			effect.audio.Play();
		}

		Const.DestroyChildrenObjects(m_weaponHolder.gameObject);

		Const.DestroyChildrenObjects(m_aimpoint);

		GameObject body = gameObject.transform.Find("Body").gameObject;
		body.transform.parent = null;
		GameObjectPool.Instance.Free(body);
		DestroyObject(gameObject);
		
		ShakeCamera(0.1f);

	}

}
