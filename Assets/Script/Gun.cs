using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	new void Start () {
		base.Start();
		m_prefBullet = Resources.Load<GameObject>("Pref/GunBullet");
	}
}
