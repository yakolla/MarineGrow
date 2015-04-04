using UnityEngine;

public class EffectTargetingPoint {

	GameObject	m_prefEffectTargetPoint = Resources.Load<GameObject>("Pref/ef_targeting");
	
	GameObject	m_effectTargetPoint;

	public void	Init(Vector3 targetPos)
	{
		targetPos.y = 0;

		if (m_effectTargetPoint == null)
		{
			m_effectTargetPoint = GameObject.Instantiate (m_prefEffectTargetPoint, targetPos, m_prefEffectTargetPoint.transform.localRotation) as GameObject;
		}

		SetActive(true);
		m_effectTargetPoint.transform.position = targetPos;
		ParticleSystem particle = m_effectTargetPoint.GetComponent<ParticleSystem>();
		particle.Play();
	}

	public void Death()
	{
		GameObject.DestroyObject(m_effectTargetPoint);
	}

	public void SetActive(bool active)
	{
		m_effectTargetPoint.SetActive(active);
	}
}
