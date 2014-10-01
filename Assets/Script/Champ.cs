using UnityEngine;
using System.Collections;

public class Champ : MonoBehaviour {

	// Use this for initialization
	NavMeshAgent	m_navAgent;
	GameObject		m_prefBullet;
	GameObject		m_aimpoint;

	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_prefBullet = Resources.Load<GameObject>("Pref/Bullet");
		m_aimpoint = this.transform.Find("Weapon/Aimpoint").gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		FollowChampWithCamera();


		if (Input.GetMouseButton(0) == true)
		{
			Vector3 pos = Input.mousePosition;
			pos.z = 10;
			m_navAgent.SetDestination(Camera.main.ScreenToWorldPoint(pos));
		}
		
		if (Input.GetMouseButtonUp(1) == true)
		{
			Vector3 pos = m_aimpoint.transform.position;
			GameObject obj = Instantiate (m_prefBullet, pos, m_aimpoint.transform.rotation) as GameObject;
		}
	}

	void FollowChampWithCamera()
	{
		Vector3 myCharacterPosition = m_navAgent.transform.position;
		myCharacterPosition.y = 6;
		myCharacterPosition.z -= 3.5f;
		Camera.main.transform.position = myCharacterPosition;
		
	}
}
