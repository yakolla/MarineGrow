using UnityEngine;
using System.Collections;

public class Follower : Champ {


	new void Start () {
		base.Start();
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.transform.tag.CompareTo("Enemy") == 0)
		{
			Vector3 pos = other.transform.position;
			float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
			int angleIndex = (int)targetAngle/20;
			
			m_animator.Play ("hero_" + angleIndex);
			
			m_weaponHolder.transform.eulerAngles =  new Vector3(90, 0, targetAngle);
			m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StartFiring(targetAngle);
		}


		
	}

}
