using UnityEngine;
using System.Collections;

public class Champ : Creature {

	float	m_startChargeTime;
	bool	m_charging = false;
	[SerializeField]
	bool	m_enableAutoTarget = true;
	new void Start () {
		base.Start();

		m_material = transform.Find("Body").GetComponent<MeshRenderer>().material;
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
	new void Update () {
		base.Update();

		UpdateChampMovement();
		FollowChampWithCamera();

		if (m_enableAutoTarget)
		{
			if (Input.GetMouseButton(1) == true)
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				
				RotateChampToPos(pos);
			}
			
			if (Input.GetMouseButtonDown(1) == true)
			{
				m_charging = true;
				m_startChargeTime = Time.time;
				m_weaponHolder.GetWeapon().StopFiring();
				Debug.Log("GetMouseButtonDown");
			}
			else if (Input.GetMouseButtonUp(1) == true)
			{
				Debug.Log("GetMouseButtonUp");
				m_charging = false;
				
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				
				m_weaponHolder.GetWeapon().StartFiring(RotateChampToPos(pos), Time.time-m_startChargeTime);
			}
			
			if (m_charging == false)
			{
				if (AutoAttack() == false)
				{
					m_weaponHolder.GetWeapon().StopFiring();
				}
				
			}
		}
		else
		{
			if (Input.GetMouseButton(1) == true)
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				
				m_weaponHolder.GetWeapon().StartFiring(RotateChampToPos(pos), 0);
			}
			else
			{
				m_weaponHolder.GetWeapon().StopFiring();
			}
		}

	}

	void FollowChampWithCamera()
	{
		Vector3 myCharacterPosition = m_navAgent.transform.position;
		myCharacterPosition.y = 6;
		myCharacterPosition.z -= 3.5f;
		Camera.main.transform.position = myCharacterPosition;
		
	}
	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("ItemBox") == 0)
		{
			ItemBox itemBox = other.gameObject.GetComponent<ItemBox>();
			switch(itemBox.ItemType)
			{
			case ItemBox.Type.HPPosion:
				m_creatureProperty.Heal(itemBox.ItemValue);
				break;
			}

			itemBox.Death();
		};

	}
}
