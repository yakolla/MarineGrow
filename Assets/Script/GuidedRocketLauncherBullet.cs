using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : RocketLauncherBullet {

	[SerializeField]
	float m_searchCoolTime = 0.3f;
	float m_lastSearchTime = 0f;
	Creature	m_target = null;

	float	m_destAngle = 0f;
	float	m_angleElpased = 1f;
	float	m_srcAngle = 0f;

	[SerializeField]
	float	m_searchRange = 3f;

	const int maxSearch = 3;
	int		m_searchCount = 0;
	// Use this for initialization
	void Start () {

	}

	new void OnEnable()
	{
		base.OnEnable();

		m_lastSearchTime = 0f;
		m_target = null;
		m_destAngle = transform.rotation.eulerAngles.y;
		m_angleElpased = 0f;
		m_searchCount = 0;
		m_srcAngle = transform.rotation.eulerAngles.y;
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

			if (m_target != null)
			{
				m_srcAngle = transform.eulerAngles.y;
				m_destAngle = -Mathf.Atan2(m_target.transform.position.z-transform.position.z, m_target.transform.position.x-transform.position.x) * Mathf.Rad2Deg;
				if (Random.Range(0, 2) == 0)
				{
					m_srcAngle -= 360;
				}

				m_angleElpased = 0f;
			}

			++m_searchCount;
		}

		if (m_angleElpased < 1f)
		{
			transform.eulerAngles = Vector3.Lerp(new Vector3(0, m_srcAngle, 0), new Vector3(0, m_destAngle, 0), m_angleElpased);
		}

		transform.Translate(Mathf.Clamp(m_accel, 0, 0.1f), 0, 0, transform);
		m_accel += Time.deltaTime*0.01f*m_speed;
		m_angleElpased += Time.deltaTime;
	}

}
