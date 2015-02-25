using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : Bullet {

	[SerializeField]
	float m_speed = 10f;
	[SerializeField]
	float m_searchCoolTime = 0.3f;
	float m_lastSearchTime = 0f;
	BoxCollider m_collider;
	GameObject m_particleSystem;
	float	m_accel = 0f;
	GameObject	m_target = null;
	// Use this for initialization
	void Start () {
		m_collider = GetComponent<BoxCollider>();
		m_particleSystem = transform.Find("Body/Particle System").gameObject;
	}

	public GameObject SearchTarget(string[] targetTags, float range)
	{
		foreach(string tag in targetTags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			foreach(GameObject target in targets)
			{				
				float dist = Vector3.Distance(transform.position, target.transform.position);
				if (dist <= range)
				{
					return target;
				}
			}
		}
		
		return null;
	}

	IEnumerator DestroyParticle(GameObject obj)
	{
		yield return new WaitForSeconds (0.5f);		
		DestroyObject(obj);
		DestroyObject(gameObject);
	}

	// Update is called once per frame
	void Update () {

		if (m_collider.enabled == false)
			return;

		if (m_target == null && m_lastSearchTime <= Time.time)
		{
			m_target = SearchTarget(m_ownerCreature.GetAutoTargetTags(), 3f);
			m_lastSearchTime = Time.time + m_searchCoolTime;

			if (m_target != null)
			{
				float angle = -Mathf.Atan2(m_target.transform.position.z-transform.position.z, m_target.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, angle, 0);

			}
		}

		transform.Translate(Mathf.Clamp(m_accel, 0, 0.1f), 0, 0, transform);
		m_accel += Time.deltaTime*0.1f*m_speed;

	}


	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, m_damageBuffType, PrefDamageEffect));

			if (m_particleSystem.transform.parent != null)
			{
				m_particleSystem.transform.parent = null;
				StartCoroutine(DestroyParticle(m_particleSystem));

			}

			this.enabled = false;
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{

			if (m_particleSystem.transform.parent != null)
			{
				m_particleSystem.transform.parent = null;
				StartCoroutine(DestroyParticle(m_particleSystem));
			}				

			this.enabled = false;
		}
	}
}
