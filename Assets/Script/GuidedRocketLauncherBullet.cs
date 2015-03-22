using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : RocketLauncherBullet {

	[SerializeField]
	float m_searchCoolTime = 0.3f;
	float m_lastSearchTime = 0f;
	BoxCollider m_collider;
	GameObject	m_target = null;

	float	m_destAngle = 0f;
	float	m_angleElpased = 1f;
	float	m_srcAngle = 0f;

	[SerializeField]
	float	m_searchRange = 3f;

	// Use this for initialization
	void Start () {
		m_collider = GetComponent<BoxCollider>();
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

		if (m_collider.enabled == false)
			return;

		if (m_target == null && m_lastSearchTime <= Time.time)
		{
			m_target = SearchTarget(m_ownerCreature.GetAutoTargetTags(), m_searchRange);
			m_lastSearchTime = Time.time + m_searchCoolTime;

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
		}

		if (m_angleElpased < 1f)
		{
			transform.eulerAngles = Vector3.Lerp(new Vector3(0, m_srcAngle, 0), new Vector3(0, m_destAngle, 0), m_angleElpased);
		}

		transform.Translate(Mathf.Clamp(m_accel, 0, 0.1f), 0, 0, transform);
		m_accel += Time.deltaTime*0.1f*m_speed;
		m_angleElpased += Time.deltaTime;
	}

}
