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
	Rect windowRect = new Rect(0, 0, 100, 100);
	void OnGUI()
	{
		
		windowRect = GUI.Window (0, windowRect, DisplayInventoryWindow, "Status");
		
	}
	
	//Setting up the Inventory window
	void DisplayInventoryWindow(int windowID)
	{
		GUI.Label(new Rect(0, 0, 30, 30), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(30, 0, 30, 30), "10");
		GUI.Label(new Rect(0, 30, 30, 30), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(0, 60, 30, 30), Resources.Load<Texture>("Sprites/robeofpower"));
	}
}
