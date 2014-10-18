using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	new void Awake () {
		base.Awake();
		m_prefBullet = Resources.Load<GameObject>("Pref/GunBullet");
	}
}
