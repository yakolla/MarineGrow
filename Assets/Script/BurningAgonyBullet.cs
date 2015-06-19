using UnityEngine;
using System.Collections;

public class BurningAgonyBullet : Bullet {

	[SerializeField]
	float	m_damageOnTime = 0.3f;

	float			m_lastDamageTime = 0f;
	int				m_lastFrame = 0;
	BoxCollider		m_collider;
	float			m_firingStartTime;

	Weapon			m_weapon;

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, weapon, targetAngle);

		m_weapon = weapon;
		transform.parent = ownerCreature.WeaponHolder.transform;
		Vector3 pos = ownerCreature.transform.position-ownerCreature.WeaponHolder.transform.position;
		pos.y = 0;
		transform.localPosition = pos;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.angle, 0));
		transform.localScale = scale;
		
	}


	override public void StartFiring()
	{
		base.StartFiring();
		m_firingStartTime = Time.time;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
	}
	
	void OnTriggerStay(Collider other) 
	{
		if (m_lastDamageTime+(m_damageOnTime*m_ownerCreature.m_creatureProperty.AttackCoolTime)<Time.time)
		{
			m_lastFrame = Time.frameCount;
			m_lastDamageTime = Time.time;
			
			TryToSetDamageBuffType(m_weapon);
			
		}
		
		if (m_lastFrame == Time.frameCount)
		{
			Creature creature = other.gameObject.GetComponent<Creature>();
			if (creature && Creature.IsEnemy(creature, m_ownerCreature))
			{
				GiveDamage(creature);
			}
		}
	}
}
