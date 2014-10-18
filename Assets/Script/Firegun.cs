using UnityEngine;
using System.Collections;

public class FireGun : Weapon {

	new void Awake () {
		base.Awake();
		m_prefBullet = Resources.Load<GameObject>("Pref/FireGunBullet");
	}

	override public void CreateBullet()
	{
		if (m_firing == false)
		{
			base.CreateBullet();
		}
	}
}
