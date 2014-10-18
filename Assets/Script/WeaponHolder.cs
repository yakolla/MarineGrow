using UnityEngine;
using System.Collections;

public class WeaponHolder : MonoBehaviour {

	Weapon	m_weapon = null;


	public void ChangeWeapon(string name)
	{
		if (m_weapon != null)
		{
			DestroyObject(m_weapon);
			m_weapon = null;
		}
		GameObject pref = Resources.Load<GameObject>("Pref/"+name);
		GameObject obj = Instantiate (pref, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		m_weapon = (Weapon)obj.GetComponent(name);

		obj.transform.parent = this.transform;
		obj.transform.localPosition = pref.transform.localPosition;
		obj.transform.localRotation = pref.transform.localRotation;
		obj.transform.localScale = pref.transform.localScale;
	}

	public Weapon GetWeapon()
	{
		return m_weapon;
	}
}
