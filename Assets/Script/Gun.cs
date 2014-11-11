using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	GameObject		m_prefAimpointEffect = null;

	GameObject		m_aimpointEffect;

	override public GameObject CreateBullet(float chargingTime)
	{
		GameObject bullet = base.CreateBullet(chargingTime);

		if (m_aimpointEffect)
		{
			m_aimpointEffect.SetActive(true);
		}
		else
		{
			if (m_prefAimpointEffect)
			{
				m_aimpointEffect = Instantiate (m_prefAimpointEffect, Vector3.zero, transform.rotation) as GameObject;
				m_aimpointEffect.transform.parent = m_aimpoint.transform;
				m_aimpointEffect.transform.localPosition = Vector3.zero;
				m_aimpointEffect.SetActive(false);
			}

		}
		
		return bullet;
	}

	override public void StopFiring()
	{
		base.StopFiring();

		if (m_aimpointEffect)		m_aimpointEffect.SetActive(false);
	}
}
