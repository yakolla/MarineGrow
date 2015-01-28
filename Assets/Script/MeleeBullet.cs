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

			RaycastHit hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			if (Physics.Raycast(transform.position, fwd, out hit, 10f))
			{
				Creature creature = hit.transform.gameObject.GetComponent<Creature>();
				if (creature && Creature.IsEnemy(creature, m_ownerCreature))
				{				
					float dist = Vector3.Distance(m_ownerCreature.transform.position, creature.transform.position);

					if (dist < 2f)
					{
						creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Fire, m_damageBuffType, PrefDamageEffect));	
						
						DestroyObject(gameObject);
					}

				}
			}
		}
		else
		{
			m_collider.enabled = false;
		}
	}
	
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, gunPoint, damage, targetAngle);
		
		transform.parent = m_gunPoint.transform;
		transform.localPosition = Vector3.zero;
		transform.localScale = scale;
	}

	void OnTriggerEnter(Collider other) 
	{
		return;
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Fire, m_damageBuffType, PrefDamageEffect));			
		}
	}
}
