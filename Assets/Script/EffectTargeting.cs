using UnityEngine;

public class EffectTargeting {
	
	protected GameObject	m_effectTargetPoint;
	protected string		m_prefName;

	float		m_lifeTime = 5f;
	float		m_startTime;

	virtual public void	Init(Vector3 targetPos)
	{
		if (m_effectTargetPoint == null)
		{
			GameObject	prefEffectTargetPoint = Resources.Load<GameObject>(m_prefName);
			m_effectTargetPoint = GameObject.Instantiate (prefEffectTargetPoint, targetPos, prefEffectTargetPoint.transform.localRotation) as GameObject;
		}

		SetActive(true);
		m_effectTargetPoint.transform.position = targetPos;

		m_startTime = Time.time + m_lifeTime;
	}

	public void Death()
	{
		SetActive(false);
		GameObject.DestroyObject(m_effectTargetPoint);
	}

	public void SetActive(bool active)
	{
		m_effectTargetPoint.SetActive(active);
	}

	void Update()
	{
		if (m_startTime < Time.time)
		{
			Death();
		}
	}
}
