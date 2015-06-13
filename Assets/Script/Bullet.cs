using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {

	protected bool 			m_isDestroying = false;
	protected Vector3		m_gunPoint;
	bool					m_firing = false;
	int						m_damage;
	protected 				Weapon.FiringDesc		m_targetAngle;
	protected				Creature	m_ownerCreature;

	Weapon					m_onHitWeapon;

	[SerializeField]
	GameObject 				m_prefDamageEffect = null;

	protected DamageDesc.Type	m_damageType = DamageDesc.Type.Normal;

	[SerializeField]
	protected DamageDesc.BuffType m_damageBuffType = DamageDesc.BuffType.Nothing;

	virtual public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		m_gunPoint = weapon.GunPointPos;
		m_ownerCreature = ownerCreature;
		m_damage = weapon.Damage;
		m_targetAngle = targetAngle;
		m_isDestroying = false;
		m_onHitWeapon = weapon.GetSubWeapon();

		m_damageBuffType = ownerCreature.m_creatureProperty.RandomWeaponBuff;

		StartFiring();
	}

	static public Creature[] SearchTarget(Vector3 pos, Creature.Type targetTags, float range)
	{

		Collider[] hitColliders = Physics.OverlapSphere(pos, range, 1<<9);
		if (hitColliders.Length == 0)
			return null;

		Creature[] testSearchedTargets = new Creature[hitColliders.Length];
		int i = 0;
		int searchedCount = 0;
		while (i < hitColliders.Length) {
				
			Creature creature = hitColliders[i].gameObject.GetComponent<Creature>();
			if (creature != null)
			{
				if (targetTags == creature.CreatureType)
				{
					testSearchedTargets[searchedCount] = creature;
					++searchedCount;
				}
			}
				
			i++;
		}

		if (searchedCount == 0)
			return null;

		Creature[] searchead = new Creature[searchedCount];
		for(i = 0; i < searchedCount; ++i)
			searchead[i] = testSearchedTargets[i];

		return searchead;
	}


	IEnumerator destoryBombObject(GameObject bombEffect, float duration)
	{
		yield return new WaitForSeconds (duration);
		GameObjectPool.Instance.Free(this.gameObject);
		DestroyObject(bombEffect);
	}

	virtual protected void bomb(float bombRange, GameObject prefBombEffect)
	{
		m_isDestroying = true;
		bombRange += m_ownerCreature.m_creatureProperty.SplashRadius;

		Creature[] searchedTargets = SearchTarget(transform.position, m_ownerCreature.GetMyEnemyType(), bombRange*0.5f);
		if (searchedTargets != null)
		{
			foreach(Creature creature in searchedTargets)
			{
				GiveDamage(creature);
			}
		}

		Vector3 bombPos = transform.position;
		bombPos.y = prefBombEffect.transform.position.y;
		
		GameObject bombEffect = (GameObject)Instantiate(prefBombEffect, bombPos, prefBombEffect.transform.rotation);
		ParticleSystem[] particleSystems = bombEffect.GetComponentsInChildren<ParticleSystem>();
		bombEffect.particleSystem.maxParticles = (int)bombRange;
		float duration = 0;
		foreach(ParticleSystem ps in particleSystems)
		{
			ps.startSize *= bombRange;
			if (duration < ps.duration)
				duration = ps.duration;
		}
		this.audio.Play();
		StartCoroutine(destoryBombObject(bombEffect, duration));
	}

	virtual public void StartFiring()
	{
		m_firing = true;

	}

	virtual public void StopFiring()
	{
		m_firing = false;
	}

	public bool IsFiring()
	{
		return m_firing;
	}

	protected void GiveDamage(Creature target)
	{
		target.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, m_damageType, m_damageBuffType, PrefDamageEffect));

		if (m_onHitWeapon != null)
		{
			if(m_onHitWeapon.canConsumeSP())
			{
				m_onHitWeapon.CreateBullet(m_targetAngle, target.transform.position);
				m_onHitWeapon.ConsumeSP();
			}
		}
	}

	public int Damage
	{
		set {m_damage = value;}
	}

	protected GameObject PrefDamageEffect
	{
		get {return m_prefDamageEffect;}
	}

}
