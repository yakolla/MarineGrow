using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	public float	m_damageOnTime = 0.3f;
	float			m_lastDamageTime = 0f;
	BoxCollider		m_collider;
	// Use this for initialization
	void Start () 
	{
		m_collider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_lastDamageTime+m_damageOnTime<Time.time)
		{
			m_collider.enabled = true;
			m_lastDamageTime = Time.time;
		}
		else
		{
			m_collider.enabled = false;
		}

	}

	override public void Init(Creature ownerCreature, GameObject aimpoint, string targetTagName, float damage)
	{
		base.Init(ownerCreature, aimpoint, targetTagName, damage);
		this.transform.parent = m_aimpoint.transform;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(this.gameObject);
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag.CompareTo(m_targetTagName) == 0)
		{
			Creature creature = other.gameObject.GetComponent<Creature>();
			creature.TakeDamage(m_ownerCreature, m_ownerCreature.m_creatureProperty.PAttackDamage);
		}
	}
}
