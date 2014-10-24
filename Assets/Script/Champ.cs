using UnityEngine;
using System.Collections;

public class Champ : MonoBehaviour {

	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected GameObject		m_weaponHolder;
	protected Animator		m_animator;
	protected GameObject		m_body;
	public float	m_coolTimeForAutoTarget = 0.5f;
	float	m_lastAutoTargetTime = 0f;
	public GameObject	m_prefWeapon;

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();

		m_animator = this.transform.Find("Body").gameObject.GetComponent<Animator> ();
		m_body = this.transform.Find("Body").gameObject;
		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject;
		m_weaponHolder.GetComponent<WeaponHolder>().ChangeWeapon(m_prefWeapon);
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
	void RotateChampToPos(Vector3 pos)
	{
		float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
		transform.eulerAngles =  new Vector3(0, -targetAngle, 0);
		m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StartFiring(targetAngle);
	}
	// Update is called once per frame
	void Update () {

		UpdateChampMovement();
		FollowChampWithCamera();

		if (Input.GetMouseButton(1) == true)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			RotateChampToPos(pos);
		}
		else
		{
			if (AutoTargetAttackableEnemy() == false)
			{
				m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StopFiring();
			}

		}


	}

	bool AutoTargetAttackableEnemy() {
		if (m_lastAutoTargetTime+m_coolTimeForAutoTarget < Time.time)
		{
			m_lastAutoTargetTime = Time.time;

			GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject enemy in enemys)
			{
				float dist = Vector3.Distance(transform.position, enemy.transform.position);
				if (dist < 3f)
				{
					Vector3 pos = enemy.transform.position;
					RotateChampToPos(pos);
					return true;
				}
			}
			
			return false;
		}

		return true;
	}

	void FollowChampWithCamera()
	{
		Vector3 myCharacterPosition = m_navAgent.transform.position;
		myCharacterPosition.y = 6;
		myCharacterPosition.z -= 3.5f;
		Camera.main.transform.position = myCharacterPosition;
		
	}
}
