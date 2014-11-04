using UnityEngine;
using System.Collections;

public class WeaponHolder : MonoBehaviour {

	Weapon			m_weapon = null;

	public void ChangeWeapon(GameObject prefWeapon, string targetTagName)
	{
		if (m_weapon != null)
		{
			DestroyObject(m_weapon);
			m_weapon = null;
		}
		GameObject obj = Instantiate (prefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
<<<<<<< HEAD
		m_weapon = (Weapon)obj.GetComponent(prefWeapon.name);
		m_weapon.m_targetTagName = targetTagName;
=======
		m_weapon = obj.GetComponent<Weapon>();
<<<<<<< HEAD
>>>>>>> origin/master
=======
		m_weapon.m_targetTagName = targetTagName;
>>>>>>> parent of 1fd5f8f... 우클릭 하고 있으면, 차징됨.

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
