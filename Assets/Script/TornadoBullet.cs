using UnityEngine;
using System.Collections;

public class TornadoBullet : Bullet {

	[SerializeField]
	float	m_speed = 3f;
	float	m_lifeTime = 4f;
	float	m_elapsed;
	// Use this for initialization
	void Start () {
		m_elapsed = Time.time + m_lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_elapsed < Time.time)
		{
			DestroyObject(gameObject);
			return;
		}
		transform.Translate(m_speed*Time.deltaTime, 0, 0, transform);
	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			DestroyObject(this.gameObject);
		}
	}
}
