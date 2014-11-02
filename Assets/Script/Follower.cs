using UnityEngine;
using System.Collections;

public class Follower : Champ {

	float	m_destroyTime;
	new void Start()
	{
		base.Start();
		m_destroyTime = Time.time+5f;
	}

	// Update is called once per frame
	new void Update () {
		if (this == null)
			return;

		if (AutoAttack() == false)
		{
			m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StopFiring();
		}

		if (m_destroyTime<Time.time)
		{
			Death();
		}
	}

}
