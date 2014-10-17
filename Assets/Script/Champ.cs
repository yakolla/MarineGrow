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
	public string			m_weaponName = "RocketLauncher";

	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		switch(m_weaponName)
		{
		case "Firegun":
			m_prefBullet = Resources.Load<GameObject>("Pref/Firegun");
			break;
		case "Bullet":
			m_prefBullet = Resources.Load<GameObject>("Pref/Bullet");
			break;
		case "RocketLauncher":
			m_prefBullet = Resources.Load<GameObject>("Pref/RocketLauncher");
			break;
		}

		m_weapon = this.transform.Find("Weapon").gameObject;
		m_aimpoint = this.transform.Find("Weapon/Aimpoint").gameObject;
		m_animator = this.transform.Find("Body").gameObject.GetComponent<Animator> ();
		m_body = this.transform.Find("Body").gameObject;

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


		if (Input.GetMouseButtonUp(1) == true)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
			int angleIndex = (int)targetAngle/20;
			
			m_animator.Play ("hero_" + angleIndex);
			m_weapon.transform.eulerAngles =  new Vector3(90, 0, targetAngle); 
			
			Debug.Log("hero_" + angleIndex);

			pos = m_aimpoint.transform.position;
			GameObject obj = Instantiate (m_prefBullet, pos, m_weapon.transform.rotation) as GameObject;
			Weapon weapon = (Weapon)obj.GetComponent(m_weaponName);
			weapon.Init(m_aimpoint);
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
