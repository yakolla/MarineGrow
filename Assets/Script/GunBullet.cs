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
		if (other.tag.CompareTo(m_targetTagName.ToString()) == 0)
		{
			DestroyObject(this.gameObject);
			Creature creature = other.gameObject.GetComponent<Creature>();
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage, DamageDesc.Type.Normal, PrefDamageEffect));
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			DestroyObject(this.gameObject);
		}
	}
}
