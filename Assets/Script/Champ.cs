using UnityEngine;
using System.Collections;

public class Champ : Creature {

	float	m_startChargeTime;
	bool	m_charging = false;

	[SerializeField]
	bool	m_enableAutoTarget = true;

	[SerializeField]
	Vector3	m_cameraOffset;

	[SerializeField]
	GameObject	m_prefLevelUpEffect = null;

	[SerializeField]
	RefCreatureBaseProperty	m_creatureBaseProperty;

	new void Start () {
		
		m_creatureProperty.init(this, m_creatureBaseProperty);
		m_creatureProperty.Level = 1;

		base.Start();

		SetFollowingCamera(null);
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);

		//m_material = transform.Find("Body").GetComponent<MeshRenderer>().material;
	}


	void LevelUp()
	{
		GameObject effect = (GameObject)Instantiate(m_prefLevelUpEffect);
		effect.transform.parent = transform;
		effect.transform.localPosition = m_prefLevelUpEffect.transform.position;
		effect.transform.localRotation = m_prefLevelUpEffect.transform.rotation;
		StartCoroutine(UpdateLevelUpEffect(effect));
		
		transform.Find("LevelupGUI").gameObject.SetActive(true);
		ChampLevelupGUI levelupGUI = transform.Find("LevelupGUI").GetComponent<ChampLevelupGUI>();
	}

	IEnumerator UpdateLevelUpEffect(GameObject effect)
	{
		yield return new WaitForSeconds(effect.particleSystem.duration);
		DestroyObject(effect);
	} 

	void UpdateChampMovement()
	{
		if (HasCrowdControl())
			return;

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

			if (m_targeting == null)
			{
				float targetHorAngle = Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);

			}

			m_navAgent.SetDestination(transform.position+pos);
		}

	}

	// Update is called once per frame
	new void Update () {
		base.Update();

		UpdateChampMovement();

		if (m_enableAutoTarget)
		{
			if (Input.GetMouseButton(1) == true)
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				
				RotateToTarget(pos);
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
				
				m_weaponHolder.GetWeapon().StartFiring(RotateToTarget(pos), m_firingDescs);
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
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				pos = ray.origin + (ray.direction* 10f);
				m_weaponHolder.GetWeapon().StartFiring(RotateToTarget(pos), m_firingDescs);
			}
			else
			{
				m_weaponHolder.GetWeapon().StopFiring();
			}
		}

		TimeEffector.Instance.Update();
	}

	override public string[] GetAutoTargetTags()
	{
		return new string[]{Creature.Type.Mob.ToString()};
	}

	override public void Death()
	{
		base.Death();
		GameObject.Find("Dungeon").GetComponent<Dungeon>().DelayLoadLevel(2);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("ItemBox") == 0)
		{
			ItemBox itemBox = other.gameObject.GetComponent<ItemBox>();
			itemBox.Pickup(this);
		};

	}
}
