using UnityEngine;
using System.Collections;

public class RocketLauncherBullet : Bullet {

	[SerializeField]
	float m_speed = 1f;
	float	m_accel = 0f;
	// Use this for initialization
	void Start () {
	}


	// Update is called once per frame
	void Update () {

		transform.Translate(m_accel, 0, 0, transform);
		m_accel += Time.deltaTime*0.1f*m_speed;


	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, m_damageBuffType, PrefDamageEffect));
			DestroyObject(this.gameObject);
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			DestroyObject(this.gameObject);
		}
	}
}
