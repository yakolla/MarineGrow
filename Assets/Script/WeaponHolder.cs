﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {

	List<Weapon>					m_weapons = new List<Weapon>();
	Dictionary<string, Weapon>		m_passiveWeapons = new Dictionary<string, Weapon>();

	float					m_weaponChangeCoolTime = 15f;
	float					m_weaponChangedTime = 0f;
	int						m_curWeaponIndex = 0;

	[SerializeField]
	bool					m_multipleWeapon = false;

	Creature				m_creature;

	bool					m_firing = false;

	void Update()
	{
		if (m_weaponChangedTime+m_weaponChangeCoolTime < Time.time)
		{
			m_weaponChangedTime = Time.time;
			m_curWeaponIndex = (m_curWeaponIndex + 1) % m_weapons.Count;
		}

		foreach(KeyValuePair<string, Weapon> pair in m_passiveWeapons)
		{
			pair.Value.StartFiring(-transform.rotation.eulerAngles.y);
		}
	}

	public void Init()
	{
		m_curWeaponIndex = 0;
		m_weaponChangedTime = 0;
		m_weapons.Clear();
		m_firing = false;

		m_creature = transform.parent.GetComponent<Creature>();
	}

	public void EquipWeapon(Weapon weapon)
	{
		m_weapons.Add(weapon);
	}

	public void EquipPassiveWeapon(Weapon weapon)
	{
		weapon.SpPerLevel = 10;
		m_passiveWeapons.Add(weapon.WeaponName, weapon);
	}

	public Weapon GetPassiveWeapon(string weaponName)
	{
		Weapon weapon = null;
		m_passiveWeapons.TryGetValue(weaponName, out weapon);

		return weapon;
	}

	public void StartFiring(float targetAngle)
	{

		if (m_multipleWeapon == false)
		{
			m_weapons[m_curWeaponIndex].StartFiring(targetAngle);
		}
		else
		{
			foreach(Weapon weapon in m_weapons)
			{
				weapon.StartFiring(targetAngle);
			}
		}

		m_firing = true;
	}

	public void StopFiring()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.StopFiring();
		}

		m_firing = false;
	}

	public void Evolution()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.Evolution();
		}
	}

	public void MoreFire()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.MoreFire();
		}
	}

	public void LevelUp()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.LevelUp();
		}
	}

	public float AttackRange()
	{
		if (m_weapons.Count == 0)
			return 0f;

		return m_weapons[m_curWeaponIndex].AttackRange;
	}

	public Weapon MainWeapon
	{
		get {return m_weapons[0];}
	}

}
