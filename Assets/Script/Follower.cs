using UnityEngine;
using System.Collections;

public class Follower : Creature {

	[SerializeField]
	float	m_destroyTime;

	ItemObject	m_weapon;

	Creature	m_champ;

	new void Start()
	{
		base.Start();
		m_destroyTime += Time.time;

		m_weapon = new ItemObject(new ItemWeaponData(103));
		m_weapon.Item.Use(this);

		m_champ = GameObject.Find("Champ(Clone)").GetComponent<Creature>();
	}

	// Update is called once per frame
	new void Update () {

		if (AutoAttack() == false)
		{
			m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StopFiring();
			this.m_navAgent.SetDestination(m_champ.transform.position);
		}

		if (m_destroyTime<Time.time)
		{
			Death();
		}


	}

	override public void GiveExp(int exp)
	{
		m_champ.GiveExp(exp);
	}

	override public string[] GetAutoTargetTags()
	{
		return new string[]{Creature.Type.Enemy.ToString()};
	}

}
