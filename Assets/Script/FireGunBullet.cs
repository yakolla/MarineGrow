using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	public float	m_damageOnTime = 0.3f;

	float			m_lastDamageTime = 0f;
	BoxCollider		m_collider;

	ParticleSystem	m_particleSystem;

	// Use this for initialization
	void Start () 
	{
		m_collider = GetComponent<BoxCollider>();
		m_particleSystem = transform.Find("Body/Particle System").particleSystem;

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
		/*
		RaycastHit hit;
		Vector3 fwd = transform.TransformDirection(Vector3.right);
		if (Physics.Raycast(transform.position, fwd, out hit, 10f))
		{
			Creature creature = hit.transform.gameObject.GetComponent<Creature>();
			if (creature && Creature.IsEnemy(creature, m_ownerCreature))
			{				
				float dist = Mathf.Min(Vector3.Distance(m_ownerCreature.transform.position, creature.transform.position)+2, 8f);
				m_particleSystem.startSpeed = dist;

				m_collider.center = new Vector3(dist/2.6f, m_collider.center.y, m_collider.center.z);
				m_collider.size = new Vector3(dist, m_collider.size.y, m_collider.size.z);

				if (m_collider.enabled)
				{
					creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Fire, m_damageBuffType, PrefDamageEffect));	
				}

			}
		}
		else{
			m_particleSystem.startSpeed = 8f;
		}*/
	}

	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, gunPoint, damage, targetAngle);

		transform.parent = m_gunPoint.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.x, 0));
		transform.localScale = scale;

	}

	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(this.gameObject);
	}

	void OnTriggerEnter(Collider other) 
	{

		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			float dist = Vector3.Distance(m_ownerCreature.transform.position, creature.transform.position);
			//m_particleSystem.startSpeed = dist;

			m_collider.center = new Vector3(dist/2.6f, m_collider.center.y, m_collider.center.z);
			m_collider.size = new Vector3(dist, m_collider.size.y, m_collider.size.z);
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Fire, m_damageBuffType, PrefDamageEffect));			
		}
	}
}
