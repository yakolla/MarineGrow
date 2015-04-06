using UnityEngine;

public class EffectTargetingPoint {


	
	GameObject	m_effectTargetPoint;

	public void	Init(Vector3 targetPos)
	{

		targetPos.y = 0;

		if (m_effectTargetPoint == null)
		{
			GameObject	prefEffectTargetPoint = Resources.Load<GameObject>("Pref/ef_targeting");
			m_effectTargetPoint = GameObject.Instantiate (prefEffectTargetPoint, targetPos, prefEffectTargetPoint.transform.localRotation) as GameObject;
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
