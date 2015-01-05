using UnityEngine;
using System.Collections;

public class FollowingCamera : MonoBehaviour
{

	[SerializeField]
	GameObject m_target;
	GameObject m_nextTarget;

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

	void Start()
	{

	}
	
	void Update()
	{
		if (m_target == null)
			return;

		m_elapsedTime += 0.01f;
		m_elapsedTime = Mathf.Min(1f, m_elapsedTime);

		Vector3 myCharacterPosition = Vector3.Lerp(m_from, m_target.transform.position-m_cameraOffset, m_elapsedTime);
		Camera.main.transform.position = myCharacterPosition;

		if (m_elapsedTime == 1f)
		{
			if (m_nextTarget != null)
			{
				SetTarget(m_nextTarget, null);
			}
		}
	}
}