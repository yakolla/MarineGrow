using UnityEngine;
using System.Collections;

public class GunBullet : Bullet {

	[SerializeField]
	float	m_speed = 3f;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(m_speed*Time.deltaTime, 0, 0, transform);		
	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			DestroyObject(this.gameObject);
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Normal, m_damageBuffType, PrefDamageEffect));
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			DestroyObject(this.gameObject);
		}
	}
}
