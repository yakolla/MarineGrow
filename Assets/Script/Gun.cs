using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	GameObject		m_prefGunPointEffect = null;

	GameObject		m_gunPointEffect;

	override public GameObject CreateBullet(Vector2 targetAngle, float chargingTime)
	{
		GameObject bullet = base.CreateBullet(targetAngle, chargingTime);

		if (m_gunPointEffect)
		{
			m_gunPointEffect.SetActive(true);
		}
		else
		{
			if (m_prefGunPointEffect)
			{
				m_gunPointEffect = Instantiate (m_prefGunPointEffect, Vector3.zero, transform.rotation) as GameObject;
				m_gunPointEffect.transform.parent = m_gunPoint.transform;
				m_gunPointEffect.transform.localPosition = Vector3.zero;
				m_gunPointEffect.SetActive(false);
			}

		}
		
		return bullet;
	}

	override public void StopFiring()
	{
		base.StopFiring();

		if (m_gunPointEffect)		m_gunPointEffect.SetActive(false);
	}
}
