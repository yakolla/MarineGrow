using UnityEngine;
using System.Collections;

public class Champ : MonoBehaviour {

	// Use this for initialization
	NavMeshAgent	m_navAgent;

	GameObject		m_weaponHolder;
	Animator		m_animator;
	GameObject		m_body;
	public string	m_weaponName = "RocketLauncher";

	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();

		m_animator = this.transform.Find("Body").gameObject.GetComponent<Animator> ();
		m_body = this.transform.Find("Body").gameObject;
		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject;
		m_weaponHolder.GetComponent<WeaponHolder>().ChangeWeapon(m_weaponName);
	}


	void UpdateChampMovement()
	{
		Vector3 pos = Vector3.zero;
		float step = 1f;
		if (Input.anyKey)
		{
			if(Input.GetKey(KeyCode.W))
			{
				pos.z += step;
			}
			if(Input.GetKey(KeyCode.S))
			{
				pos.z -= step;
			}
			if(Input.GetKey(KeyCode.A))
			{
				pos.x -= step;
			}
			if(Input.GetKey(KeyCode.D))
			{
				pos.x += step;
			}
			
			m_navAgent.SetDestination(transform.position+pos);
		}

	}
	// Update is called once per frame
	void Update () {

		UpdateChampMovement();
		FollowChampWithCamera();


		if (Input.GetMouseButton(1) == true)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
			int angleIndex = (int)targetAngle/20;
			
			m_animator.Play ("hero_" + angleIndex);

			m_weaponHolder.transform.eulerAngles =  new Vector3(90, 0, targetAngle);
			m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StartFiring();
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
