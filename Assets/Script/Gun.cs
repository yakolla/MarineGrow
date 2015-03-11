using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	GameObject		m_prefGunPointEffect = null;

	GameObject		m_gunPointEffect;

	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(targetAngle, startPos);

		if (m_gunPointEffect)
		{
			m_gunPointEffect.GetComponent<ParticleSystem>().Play();
			m_gunPointEffect.SetActive(true);
		}
		else
		{
			if (m_prefGunPointEffect)
			{
				m_gunPointEffect = Instantiate (m_prefGunPointEffect, Vector3.zero, transform.rotation) as GameObject;
				m_gunPointEffect.transform.parent = m_gunPoint.transform;
				m_gunPointEffect.transform.localPosition = Vector3.zero;
				m_gunPointEffect.transform.localScale = m_prefGunPointEffect.transform.localScale;
				m_gunPointEffect.transform.localRotation = m_prefGunPointEffect.transform.localRotation;
				m_gunPointEffect.SetActive(false);
			}

		}
		
		return bullet;
	}

	override public void StopFiring()
	{
		base.StopFiring();

		if (m_gunPointEffect)		
			m_gunPointEffect.SetActive(false);
	}
}
