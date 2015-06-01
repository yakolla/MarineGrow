using UnityEngine;
using System.Collections;

public class ExplosionPassiveLauncher : Weapon {

	[SerializeField]
	GameObject m_prefChargingEffect;

	ParticleSystem[] m_chargingEffect;
	float[]			m_maxSize;
	new void Start()
	{
		base.Start();

		if (m_prefChargingEffect != null)
		{
			GameObject obj = Instantiate (m_prefChargingEffect, Vector3.zero, transform.rotation) as GameObject;
			
			obj.transform.parent = m_creature.transform;
			obj.transform.localPosition = m_prefChargingEffect.transform.localPosition;
			obj.transform.localScale = m_prefChargingEffect.transform.localScale;
			obj.transform.localRotation = m_prefChargingEffect.transform.localRotation;
			m_chargingEffect = obj.GetComponentsInChildren<ParticleSystem>();
			m_maxSize = new float[m_chargingEffect.Length];

			for(int i = 0; i < m_chargingEffect.Length; ++i)
				m_maxSize[i] = m_chargingEffect[i].startSize;
		}
	}

	override public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		ExplosionBullet bullet = base.CreateBullet(targetAngle, startPos) as ExplosionBullet;
		bullet.BombRange += (m_level-1);
		return bullet;
	}

	override public void LevelUp()
	{
		++m_level;		
	}

	void Update()
	{
		if (isCoolTime())
		{
			base.StartFiring(0f);
		}
		else
		{
			for(int i = 0; i < m_chargingEffect.Length; ++i)
				m_chargingEffect[i].startSize = (remainCoolTimeRatio()+0.3f)*m_maxSize[i];
		}
	}
}
