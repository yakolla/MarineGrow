using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

	new void Awake () {
		base.Awake();
		m_prefBullet = Resources.Load<GameObject>("Pref/RocketLauncherBullet");
	}

}
