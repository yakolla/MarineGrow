using UnityEngine;
using System.Collections;

public class WeaponHolder : MonoBehaviour {

	Weapon			m_weapon = null;

	public void ChangeWeapon(Weapon weapon)
	{
		if (m_weapon != null)
		{
			DestroyObject(m_weapon);
			m_weapon = null;
		}

		m_weapon = weapon;
	}

	public Weapon GetWeapon()
	{
		return m_weapon;
	}
}
