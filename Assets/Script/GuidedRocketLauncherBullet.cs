using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : RocketLauncherBullet {

	[SerializeField]
	float m_searchCoolTime = 0.3f;
	float m_lastSearchTime = 0f;
	Creature	m_target = null;


	[SerializeField]
	float	m_searchRange = 3f;

	[SerializeField]
	int maxSearch = 3;

	float	m_refreshAngleCoolTime = 0f;


	Weapon	m_weapon;
	// Use this for initialization
	void Start () {

	}

	override public void Init(Creature ownerCreature, Weapon weapon, Weapon.FiringDesc targetAngle)
	{
		base.Init(ownerCreature, weapon, targetAngle);

		m_weapon = weapon;
	}

	new void OnEnable()
	{
		base.OnEnable();

		m_lastSearchTime = 0f;
		m_target = null;
	}

	// Update is called once per frame
	void Update () {

		if (m_isDestroying == true)
			return;

		if (m_target == null && m_lastSearchTime <= Time.time)
		{
			m_lastSearchTime = Time.time + m_searchCoolTime;

			Creature[] searchedTargets = Bullet.SearchTarget(transform.position, m_ownerCreature.GetMyEnemyType(), m_searchRange);
			if (searchedTargets != null)
				m_target = searchedTargets[Random.Range(0, searchedTargets.Length)];

		}


		float destAngle = transform.eulerAngles.y;
		if (m_target != null)
		{
			destAngle = Mathf.Atan2(m_target.transform.position.z-transform.position.z, m_target.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
		}
		transform.eulerAngles = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, -destAngle, 0)), 300f*Time.deltaTime).eulerAngles;

		//transform.Translate(Mathf.Clamp(m_accel, 0, 0.1f), 0, 0, transform);
		transform.Translate(m_accel, 0, 0, transform);
		m_accel += Time.fixedDeltaTime*Time.fixedDeltaTime*m_speed;

		if (m_target != null)
		{
			if (1.3f > Vector3.Distance(transform.position, m_target.transform.position))
			{
				Bomb();
				m_weapon.SendMessage("OnDestroyBullet");
			}
		}
	}

	new void OnTriggerEnter(Collider other) {
		if (m_isDestroying == true)
			return;

		if (other.tag.CompareTo("Wall") == 0)
		{
			GameObjectPool.Instance.Free(this.gameObject);
			m_weapon.SendMessage("OnDestroyBullet");
		}
	}

}
