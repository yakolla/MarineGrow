using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {

	List<Weapon>			m_weapons = new List<Weapon>();

	public void EquipWeapon(Weapon weapon)
	{
		m_weapons.Add(weapon);
	}

	public void StartFiring(Vector2 targetAngle)
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.StartFiring(targetAngle);
		}
	}

	public void StopFiring()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.StopFiring();
		}
	}

	public void Evolution()
	{
		foreach(Weapon weapon in m_weapons)
		{
			weapon.Evolution();
		}
	}

	public float AttackRange()
	{
		if (m_weapons.Count == 0)
			return 0f;

		return m_weapons[0].AttackRange;
	}
}
