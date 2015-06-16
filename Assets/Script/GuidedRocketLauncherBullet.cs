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
	int		m_searchCount = 0;
	// Use this for initialization
	void Start () {

	}

	new void OnEnable()
	{
		base.OnEnable();

		m_lastSearchTime = 0f;
		m_target = null;
		m_searchCount = 0;
	}

	// Update is called once per frame
	void Update () {

		if (m_isDestroying == true)
			return;

		if (m_target == null && m_lastSearchTime <= Time.time && m_searchCount < maxSearch)
		{
			m_lastSearchTime = Time.time + m_searchCoolTime;

			Creature[] searchedTargets = Bullet.SearchTarget(transform.position, m_ownerCreature.GetMyEnemyType(), m_searchRange);
			if (searchedTargets != null)
				m_target = searchedTargets[Random.Range(0, searchedTargets.Length)];

			++m_searchCount;
		}


		float destAngle = transform.eulerAngles.y;
		if (m_target != null)
		{
			destAngle = -Mathf.Atan2(m_target.transform.position.z-transform.position.z, m_target.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
			if (destAngle < 0)
			{
				destAngle += 360;
				destAngle %= 360;
			}

		}

		transform.eulerAngles = Vector3.Lerp(new Vector3(0, transform.eulerAngles.y, 0), new Vector3(0, destAngle, 0), 0.1f);

		transform.Translate(Mathf.Clamp(m_accel, 0, 0.1f), 0, 0, transform);
		m_accel += Time.deltaTime*0.01f*m_speed;

		if (m_target != null)
		{
			if (1.3f > Vector3.Distance(transform.position, m_target.transform.position))
			{
				bomb(m_bombRange, m_prefBombEffect);
			}
		}
	}

}
