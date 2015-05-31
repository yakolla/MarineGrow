using UnityEngine;
using System.Collections;

public class Follower : Creature {


	Creature	m_owner;
	MobAI		m_ai;

	// Update is called once per frame
	new void Update () {

		if (AutoAttack() == false)
		{
			m_weaponHolder.GetComponent<WeaponHolder>().StopFiring();
			if (m_owner != null)
				m_navAgent.SetDestination(m_owner.transform.position);
		}
		else
		{
			m_ai.SetTarget(m_targeting);
			//m_navAgent.SetDestination(m_targeting.transform.position);
			if (m_owner != null)
				m_navAgent.SetDestination(m_owner.transform.position);
			m_ai.Update();
		}
		base.Update();
	}

	public void Init(Creature owner, MobAIType aiType, RefCreatureBaseProperty baseProperty, int level)
	{
		base.Init();

		m_owner = owner;
		CreatureType = m_owner.CreatureType;
		m_creatureProperty.init(this, baseProperty, level);

		switch(aiType)
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

	override public void Death()
	{
		base.Death();

		Const.GetSpawn().RemoveFollower(this);
	}
}
