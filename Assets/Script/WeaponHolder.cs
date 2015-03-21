using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {

	List<Weapon>			m_weapons = new List<Weapon>();
	float					m_weaponChangeCoolTime = 15f;
	float					m_weaponChangedTime = 0f;
	int						m_curWeaponIndex = 0;

	[SerializeField]
	bool					m_conWeapon = false;

	void Update()
	{
		if (m_weaponChangedTime+m_weaponChangeCoolTime < Time.time)
		{
			m_weaponChangedTime = Time.time;
			m_curWeaponIndex = (m_curWeaponIndex + 1) % m_weapons.Count;
		}
	}

	public void EquipWeapon(Weapon weapon)
	{
		m_weapons.Add(weapon);
	}

	public void StartFiring(Vector2 targetAngle)
	{
		if (m_conWeapon == false)
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
}
