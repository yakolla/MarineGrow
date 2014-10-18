using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningBoltLauncher : Weapon {



	new void Awake () {
	
		base.Awake();
		m_prefBullet = Resources.Load<GameObject>("Pref/LightningBoltBullet");

	}

	override public void CreateBullet()
	{
		if (m_firing == false)
		{
			base.CreateBullet();
		}
	}
}
