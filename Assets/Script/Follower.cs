using UnityEngine;
using System.Collections;

public class Follower : Creature {


	Creature	m_owner;
	MobAI		m_ai;
	int			m_refMobId;

	// Update is called once per frame
	new void Update () {

		base.Update();

		if (AutoAttack() == false)
		{
			m_weaponHolder.GetComponent<WeaponHolder>().StopFiring();
		}

		if (m_owner != null)
			m_navAgent.SetDestination(m_owner.transform.position);
		
	}

	IEnumerator DecHpEffect()
	{
		while(m_creatureProperty.HP > 0)
		{
			yield return new WaitForSeconds(1f);
			m_creatureProperty.HP -= 1;
		}

		Death();
	}

	public void Init(Creature owner, RefMob refMob, int level)
	{
		base.Init();

		m_owner = owner;
		CreatureType = m_owner.CreatureType;
		m_refMobId = refMob.id;

		m_creatureProperty.init(this, refMob.baseCreatureProperty, level);

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
		}
		
		m_ai.Init(this);

		StartCoroutine(DecHpEffect());
	
	}

	public int FollowerID
	{
		get{return m_refMobId;}
	}

	void LevelUp()
	{
		ApplyBuff(null, DamageDesc.BuffType.LevelUp, 10f, null);
		WeaponHolder.LevelUp();
	}

	override public void GiveExp(int exp)
	{
		m_creatureProperty.giveExp((int)(exp+exp*m_owner.m_creatureProperty.GainExtraExp));
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("ItemBox") == 0)
		{
			if (3f > Vector3.Distance(transform.position, other.transform.position))
			{
				ItemBox itemBox = other.gameObject.GetComponent<ItemBox>();
				itemBox.StartPickupEffect(m_owner);
			}
		};
		
	}

	override public string[] GetAutoTargetTags()
	{
		if (m_owner)
			return m_owner.GetAutoTargetTags();

		return new string[]{""};
	}

}
