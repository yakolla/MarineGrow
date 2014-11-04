using UnityEngine;
using System.Collections;

public class Champ : Creature {


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
	protected void Update () {

		UpdateChampMovement();
		FollowChampWithCamera();

		if (Input.GetMouseButton(1) == true)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			RotateChampToPos(pos);
		}
		else
		{
			if (AutoAttack() == false)
			{
				m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StopFiring();
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
}
