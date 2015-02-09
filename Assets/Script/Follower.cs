using UnityEngine;
using System.Collections;

public class Follower : Creature {

	[SerializeField]
	int	m_refItemId = 101;

	ItemObject	m_weapon;

	Creature	m_owner;
	MobAI				m_ai;
	new void Start()
	{

		base.Start();

		m_weapon = new ItemObject(new ItemWeaponData(m_refItemId, null));
		m_weapon.Item.Use(this);

	}

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
			m_navAgent.SetDestination(m_targeting.transform.position);
			m_ai.Update();
		}

	}

	public int WeaponItemID
	{
		set {m_refItemId = value;}
	}

	public void Init(Creature owner, MobAIType aiType, RefCreatureBaseProperty baseProperty)
	{
		m_owner = owner;
		if (m_owner)
		{
			CreatureType = m_owner.CreatureType;
			m_creatureProperty.init(this, baseProperty);
			Spawn = owner.Spawn;

			switch(aiType)
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
	}

	override public void GiveExp(int exp)
	{
		m_owner.GiveExp(exp);
	}

	override public string[] GetAutoTargetTags()
	{
		if (m_owner)
			return m_owner.GetAutoTargetTags();

		return new string[]{""};
	}

}
