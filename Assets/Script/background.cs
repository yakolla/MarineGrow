using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {

	NavMeshAgent	m_navAgent;
	// Use this for initialization
	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 myCharacterPosition = m_navAgent.transform.position;
		myCharacterPosition.y = 6;
		myCharacterPosition.z -= 3.5f;
		Camera.main.transform.position = myCharacterPosition;

		if (Input.GetMouseButton(0) == true)
		{
			Vector3 pos = Input.mousePosition;
			pos.z = 10;
			m_navAgent.SetDestination(Camera.main.ScreenToWorldPoint(pos));
		}
			
	}
}
