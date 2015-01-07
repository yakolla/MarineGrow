using UnityEngine;
using System.Collections;

public class FollowingCamera : MonoBehaviour
{

	[SerializeField]
	GameObject m_target;
	GameObject m_nextTarget;
	GameObject m_mainTarget;

	[SerializeField]
	Vector3 m_cameraOffset;

	Vector3	m_from;

	float m_elapsedTime = 0f;

	public void SetTarget(GameObject target, GameObject nextTarget)
	{
		m_target = target;
		m_nextTarget = nextTarget;
		m_elapsedTime = 0f;
		m_from = Camera.main.transform.position;
	}

	public void SetMainTarget(GameObject mainTarget)
	{
		m_mainTarget = mainTarget;
	}

	void Start()
	{

	}
	
	void Update()
	{
		if (m_target == null)
		{
			m_target = m_mainTarget;
			m_elapsedTime = 0f;
			m_from = Camera.main.transform.position;
			return;
		}

		m_elapsedTime += 0.01f;

		Vector3 myCharacterPosition = Vector3.Lerp(m_from, m_target.transform.position-m_cameraOffset, Mathf.Min(1f, m_elapsedTime));
		Camera.main.transform.position = myCharacterPosition;

		if (m_elapsedTime >= 2f)
		{
			if (m_nextTarget != null)
			{
				SetTarget(m_nextTarget, null);
			}
		}
	}
}