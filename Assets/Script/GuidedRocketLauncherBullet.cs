using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : Bullet {

	[SerializeField]
	float m_speed = 10f;
	[SerializeField]
	float m_searchCoolTime = 0.3f;
	float m_lastSearchTime = 0f;

	float	m_accel = 0f;
	GameObject	m_target = null;
	// Use this for initialization
	void Start () {
	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage+damage, targetAngle);

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
	// Update is called once per frame
	void Update () {

		if (m_target == null && m_lastSearchTime <= Time.time)
		{
			m_target = SearchTarget(m_ownerCreature.GetAutoTargetTags(), 3f);
			m_lastSearchTime = Time.time + m_searchCoolTime;

			if (m_target != null)
			{
				float targetHorAngle = Mathf.Atan2(m_target.transform.position.z-transform.position.z, m_target.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);
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
			DestroyObject(this.gameObject);
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			DestroyObject(this.gameObject);
		}
	}
}
