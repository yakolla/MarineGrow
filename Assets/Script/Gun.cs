using UnityEngine;
using System.Collections;

public class Gun : Weapon {

	[SerializeField]
	GameObject		m_prefGunPointEffect = null;

	ParticleSystem		m_gunPointEffect;


	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(targetAngle, startPos);


		if (m_gunPointEffect == null)
		{
			GameObject obj = Instantiate (m_prefGunPointEffect, Vector3.zero, transform.rotation) as GameObject;
			
			obj.transform.parent = m_gunPoint.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = m_prefGunPointEffect.transform.localScale;
			obj.transform.localRotation = m_prefGunPointEffect.transform.localRotation;
			
			m_gunPointEffect = obj.GetComponent<ParticleSystem>();
		}

		m_gunPointEffect.gameObject.SetActive(true);
		if (m_gunPointEffect.isStopped)
			m_gunPointEffect.Play();

		
		return bullet;
	}
}
