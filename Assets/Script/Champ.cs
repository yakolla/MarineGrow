﻿using UnityEngine;
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
			m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StartFiring(targetAngle);
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
			Debug.Log(m_lastAutoTargetTime);

			GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject enemy in enemys)
			{
				float dist = Vector3.Distance(transform.position, enemy.transform.position);
				if (dist < 3f)
				{
					Vector3 pos = enemy.transform.position;
					float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
					int angleIndex = (int)targetAngle/20;
					
					m_animator.Play ("hero_" + angleIndex);
					
					m_weaponHolder.transform.eulerAngles =  new Vector3(90, 0, targetAngle);
					m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StartFiring(targetAngle);
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
