using UnityEngine;
using System.Collections;

public class FollowingCamera : MonoBehaviour
{

	[SerializeField]
	GameObject m_target;

	[SerializeField]
	Vector3 m_cameraOffset;

	Bezier m_bezier;

	public void SetTarget(GameObject target)
	{
		Vector3 handle1 = new Vector3(transform.position.x, m_cameraOffset.y, transform.position.z);
		Vector3 handle2 = new Vector3(target.transform.position.x, m_cameraOffset.y, target.transform.position.z);
		m_target = target;
		m_bezier = new Bezier(gameObject, target, handle1, handle2, 0.01f);
	}

	void Start()
	{

	}
	
	void Update()
	{
		if (m_target == null)
			return;

		Vector3 myCharacterPosition = m_target.transform.position;
		if (m_bezier.Update() == false)
		{

		}
		else
		{
			myCharacterPosition = transform.position;
		}

		myCharacterPosition.y = m_cameraOffset.y;
		myCharacterPosition.z -= m_cameraOffset.z;
		myCharacterPosition.x -= m_cameraOffset.x;
		Camera.main.transform.position = myCharacterPosition;
	}
}