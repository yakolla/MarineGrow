using UnityEngine;
using System.Collections;

public class WeaponHolder : MonoBehaviour {

	Weapon			m_weapon = null;

	public void ChangeWeapon(GameObject prefWeapon)
	{
		if (m_weapon != null)
		{
			DestroyObject(m_weapon);
			m_weapon = null;
		}

		GameObject obj = Instantiate (prefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		m_weapon = obj.GetComponent<Weapon>();

		obj.transform.parent = this.transform;
		obj.transform.localPosition = prefWeapon.transform.localPosition;
		obj.transform.localRotation = prefWeapon.transform.localRotation;
		obj.transform.localScale = prefWeapon.transform.localScale;
	}

	public Weapon GetWeapon()
	{
		return m_weapon;
	}
}
