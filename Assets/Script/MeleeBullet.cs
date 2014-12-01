using UnityEngine;
using System.Collections;

public class MeleeBullet : Bullet {
	
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
		
		
		this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, m_targetAngle.y));
	}
	
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, gunPoint, damage, chargingTime, targetAngle);
		
		transform.parent = m_gunPoint.transform;
		transform.localPosition = Vector3.zero;
		transform.localScale = scale;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(gameObject);
	}
	
	void OnTriggerEnter(Collider other) 
	{
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Fire, PrefDamageEffect));			
		}
	}
}
