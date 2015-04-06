using UnityEngine;
using System.Collections;

public class GunBullet : Bullet {

	[SerializeField]
	float	m_speed = 3f;

	bool	m_isDestroying = false;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(m_speed*Time.deltaTime, 0, 0, transform);		
	}

	void OnTriggerEnter(Collider other) {
		if (m_isDestroying == true)
			return;

		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
			m_isDestroying = true;
			DestroyObject(this.gameObject);
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			m_isDestroying = true;
			DestroyObject(this.gameObject);
		}
	}

	public float	BulletSpeed
	{
		get {return m_speed;}
		set {m_speed = value;}
	}
}
