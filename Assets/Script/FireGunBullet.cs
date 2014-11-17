using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	public float	m_damageOnTime = 0.3f;
	[SerializeField]
	GameObject 		m_prefDamageEffect;
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

	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, chargingTime, targetAngle);
		this.transform.parent = m_gunPoint.transform;
		this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle.y));
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
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Fire, m_prefDamageEffect));			
		}
	}
}
