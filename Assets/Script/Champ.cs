using UnityEngine;
using System.Collections;

public class Champ : MonoBehaviour {

	// Use this for initialization
	NavMeshAgent	m_navAgent;
	GameObject		m_prefBullet;
	GameObject		m_aimpoint;
	Animator		m_animator;
	GameObject		m_body;
	GameObject		m_weapon;

	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_prefBullet = Resources.Load<GameObject>("Pref/Bullet");
		m_weapon = this.transform.Find("Weapon").gameObject;
		m_aimpoint = this.transform.Find("Weapon/Aimpoint").gameObject;
		m_animator = this.transform.Find("Body").gameObject.GetComponent<Animator> ();
		m_body = this.transform.Find("Body").gameObject;

	}
	
	// Update is called once per frame
	void Update () {

		FollowChampWithCamera();

		//m_body.transform.LookAt(Camera.main.transform.position, -Vector3.up);

		if (Input.GetMouseButton(0) == true)
		{
			Vector3 pos = Input.mousePosition;
			pos.z = 10;
			m_navAgent.SetDestination(Camera.main.ScreenToWorldPoint(pos));

			pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
			int angleIndex = (int)targetAngle/20;

			m_animator.Play ("hero_" + angleIndex);
			m_weapon.transform.eulerAngles =  new Vector3(90, 0, targetAngle); 

			Debug.Log("hero_" + angleIndex);



		}
		
		if (Input.GetMouseButtonUp(1) == true)
		{
			Vector3 pos = m_aimpoint.transform.position;
			GameObject obj = Instantiate (m_prefBullet, pos, m_weapon.transform.rotation) as GameObject;
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
