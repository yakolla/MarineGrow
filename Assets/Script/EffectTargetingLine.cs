using UnityEngine;

public class EffectTargetingLine {
	
	GameObject	m_effectTargetPoint;

	public void	Init(Vector3 srcPos, Vector3 targetPos)
	{
		if (m_effectTargetPoint == null)
		{
			GameObject	prefEffectTargetPoint = Resources.Load<GameObject>("Pref/ef laser point _enemy");
			m_effectTargetPoint = GameObject.Instantiate (prefEffectTargetPoint, srcPos, prefEffectTargetPoint.transform.localRotation) as GameObject;
		}

		SetActive(true);
		m_effectTargetPoint.transform.position = srcPos;

		float targetHorAngle = Mathf.Atan2(targetPos.z-srcPos.z, targetPos.x-srcPos.x) * Mathf.Rad2Deg;
		m_effectTargetPoint.transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);

		m_effectTargetPoint.transform.Translate(new Vector3(m_effectTargetPoint.transform.localScale.x, 0, 0));

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
