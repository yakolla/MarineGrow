using UnityEngine;
using System.Collections;

public class Melee : Weapon {

	MeleeBullet	m_bullet;

	override protected Weapon.FiringDesc DefaultFiringDesc()
	{
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = 0;
		desc.delayTime = 0.3f;
		
		return desc;
	}

	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(targetAngle, startPos);
		Vector3 scale = bullet.transform.localScale;
		scale.x = m_attackRange;
		bullet.transform.localScale = scale;

		return bullet;
	}
	
	override public void StartFiring(Vector2 targetAngle)
	{		
		if (m_firing == false && isCoolTime() == true )
		{
			if (null == m_bullet)
			{
				m_bullet = CreateBullet(targetAngle, m_gunPoint.transform.position) as MeleeBullet;
			}
			m_bullet.Damage = Damage;
			m_bullet.gameObject.SetActive(true);
			
			this.audio.Play();
			
		}
		if (null != m_bullet)
		{
			Vector3 euler = m_bullet.transform.rotation.eulerAngles;
			euler.y = transform.eulerAngles.y;
			m_bullet.transform.eulerAngles = euler;			
		}
		
		playGunPointEffect();
		m_firing = true;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
		this.audio.Stop();
		if (m_bullet != null)
		{
			m_bullet.StopFiring();
			m_bullet.gameObject.SetActive(false);
		}
		
		stopGunPointEffect();
		
	}
	
	override public void MoreFire()
	{

	}
	
	override public void LevelUp()
	{
		
	}
}
