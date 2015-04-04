using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponHolder : MonoBehaviour {

	List<Weapon>			m_weapons = new List<Weapon>();
	float					m_weaponChangeCoolTime = 15f;
	float					m_weaponChangedTime = 0f;
	int						m_curWeaponIndex = 0;

	[SerializeField]
	bool					m_multipleWeapon = false;

	[SerializeField]
	Vector2				m_chargingSpeed = Vector2.zero;
	
	[SerializeField]
	float				m_chargingGuage = 1f;

	[SerializeField]
	bool				m_enableCharging = true;

	bool					m_firing = false;

	void Update()
	{
		if (m_weaponChangedTime+m_weaponChangeCoolTime < Time.time)
		{
			m_weaponChangedTime = Time.time;
			m_curWeaponIndex = (m_curWeaponIndex + 1) % m_weapons.Count;
		}

		if (m_enableCharging)
		{
			if (m_firing)
			{
				m_chargingGuage -= (1-Mathf.Min(1f, m_chargingSpeed.y))*Time.deltaTime;
				m_chargingGuage = Mathf.Max(0, m_chargingGuage);

				if (m_chargingGuage == 0f)
				{
					StopFiring();
				}
			}
			else
			{
				m_chargingGuage += (m_chargingSpeed.x)*Time.deltaTime;
				m_chargingGuage = Mathf.Min(1, m_chargingGuage);
			}
		}
	}

	public void EquipWeapon(Weapon weapon)
	{
		m_weapons.Add(weapon);
	}

	public void StartFiring(Vector2 targetAngle)
	{
		if (m_chargingGuage < 0.05f)
			return;

		if (m_multipleWeapon == false)
		{
			m_weapons[m_curWeaponIndex].StartFiring(targetAngle);
			m_chargingSpeed = m_weapons[m_curWeaponIndex].ChargingSpeed;
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

	public float ChargingGuage
	{
		set{m_chargingGuage = value;}
		get{return m_chargingGuage;}
	}

	public bool EnableChargingGuage
	{
		get{return m_enableCharging;}
		set{m_enableCharging = value;}
	}
}
