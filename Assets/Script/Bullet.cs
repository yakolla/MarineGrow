using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected bool 			m_isDestroying = false;
	protected Vector3	m_gunPoint;
	bool					m_firing = false;
	int						m_damage;
	protected 	Vector2		m_targetAngle;
	protected	Creature	m_ownerCreature;

	Weapon		m_onHitWeapon;

	[SerializeField]
	GameObject 		m_prefDamageEffect = null;

	protected DamageDesc.Type	m_damageType = DamageDesc.Type.Normal;

	[SerializeField]
	protected DamageDesc.BuffType m_damageBuffType = DamageDesc.BuffType.Nothing;

	virtual public void Init(Creature ownerCreature, Vector3 gunPoint, int damage, Vector2 targetAngle, Weapon onHitWeapon)
	{
		m_gunPoint = gunPoint;
		m_ownerCreature = ownerCreature;
		m_damage = damage;
		m_targetAngle = targetAngle;
		m_isDestroying = false;
		m_onHitWeapon = onHitWeapon;

		m_damageBuffType = ownerCreature.m_creatureProperty.RandomWeaponBuff;

		StartFiring();
	}

	public GameObject SearchTarget(string[] targetTags, float range)
	{
		/*
		foreach(string tag in targetTags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			foreach(GameObject target in targets)
			{				
				float dist = Vector3.Distance(transform.position, target.transform.position);
				if (dist <= range)
				{
					return target;
				}
			}
		}
		*/

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
		int i = 0;
		while (i < hitColliders.Length) {
			foreach(string tag in targetTags)
			{
				if (true == hitColliders[i].CompareTag(tag))
				{
					return hitColliders[i].gameObject;
				}
			}
			i++;
		}
		
		return null;
	}

	IEnumerator destoryBombObject(GameObject bombEffect)
	{
		yield return new WaitForSeconds (bombEffect.particleSystem.duration);
		GameObjectPool.Instance.Free(this.gameObject);
		DestroyObject(bombEffect);
	}

	virtual protected void bomb(float bombRange, GameObject prefBombEffect)
	{
		m_isDestroying = true;
		/*
		string[] tags = m_ownerCreature.GetAutoTargetTags();
		foreach(string tag in tags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			Vector3 pos = transform.position;
			//pos.y = 0;
			foreach(GameObject target in targets)
			{
				float dist = Vector3.Distance(pos, target.transform.position);
				if (dist < bombRange/2)
				{
					Creature creature = target.GetComponent<Creature>();
					GiveDamage(creature);
				}
			}
		}
		*/

		string[] tags = m_ownerCreature.GetAutoTargetTags();
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, bombRange/2);
		int i = 0;
		while (i < hitColliders.Length) {
			foreach(string tag in tags)
			{
				if (true == hitColliders[i].CompareTag(tag))
				{
					Creature creature = hitColliders[i].gameObject.GetComponent<Creature>();
					GiveDamage(creature);
				}
			}
			i++;
		}

		Vector3 bombPos = transform.position;
		bombPos.y = prefBombEffect.transform.position.y;
		
		GameObject bombEffect = (GameObject)Instantiate(prefBombEffect, bombPos, prefBombEffect.transform.rotation);
		bombEffect.particleSystem.startSize *= bombRange*2;
		this.audio.Play();
		StartCoroutine(destoryBombObject(bombEffect));
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
			m_onHitWeapon.StartFiring(m_targetAngle);
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
