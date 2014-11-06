using UnityEngine;
using System.Collections;

public class GunBullet : Bullet {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(3*Time.deltaTime, 0, 0, transform);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == m_targetTagName)
		{
			DestroyObject(this.gameObject);
			Creature creature = other.gameObject.GetComponent<Creature>();
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PAttackDamage, DamageDesc.Type.Normal, null));
		}
		else if (other.tag == "Wall")
		{
			DestroyObject(this.gameObject);
		}
	}
}
