﻿using UnityEngine;
using System.Collections;

public class Follower : Creature {

	[SerializeField]
	int	m_refItemId = 101;

	ItemObject	m_weapon;

	Creature	m_champ;

	new void Start()
	{
		base.Start();

		
		m_creatureProperty.init(this, 0f, 0f, 0f);

		m_weapon = new ItemObject(new ItemWeaponData(m_refItemId));
		m_weapon.Item.Use(this);

		m_champ = GameObject.Find("Champ(Clone)").GetComponent<Creature>();
	}

	// Update is called once per frame
	new void Update () {

		if (AutoAttack() == false)
		{
			m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StopFiring();
			if (m_champ != null)
				m_navAgent.SetDestination(m_champ.transform.position);
		}
		else
		{
			m_navAgent.Stop();
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
