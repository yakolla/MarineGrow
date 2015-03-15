using UnityEngine;
using System.Collections;

public class BoomerangBullet : Bullet {

	[SerializeField]
	float	m_speed = 3f;
	float	m_lifeTime = 4f;
	float	m_elapsed;

	Vector3	m_start;
	Vector3	m_goal;

	bool	m_returnPhase = false;
	// Use this for initialization
	void Start () {

		m_start = transform.position;
		m_goal = m_start+transform.right*10;

		Debug.Log("start:" + m_start + ", goal:" + m_goal);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp(m_start, m_goal, m_elapsed/m_lifeTime);
		m_elapsed += Time.deltaTime;

		if (m_elapsed/m_lifeTime > 1)
		{
			if (m_returnPhase == true)
			{
				DestroyObject(gameObject);
			}
			else
			{
				m_returnPhase = true;
				m_elapsed = 0f;
				m_start = m_goal;

				if (m_ownerCreature)
					m_goal = m_ownerCreature.transform.position;
			}

		}

		if (m_returnPhase == true)
		{
			if (m_ownerCreature)
				m_goal = m_ownerCreature.transform.position;
		}
	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, DamageDesc.BuffType.Airborne, PrefDamageEffect));
		}

	}
}
