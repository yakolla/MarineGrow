using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

	new void Start () {
		base.Start();
		m_prefBullet = Resources.Load<GameObject>("Pref/RocketLauncherBullet");
	}

}
