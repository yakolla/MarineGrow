using UnityEngine;
using System.Collections;

public class Champ : Creature {

	float	m_startChargeTime;

	Joystick	m_leftJoystick;
	Joystick	m_rightJoystick;

	[SerializeField]
	bool	m_enableAutoTarget = true;

	[SerializeField]
	Vector3	m_cameraOffset;

	[SerializeField]
	GameObject	m_prefLevelUpEffect = null;

	[SerializeField]
	RefCreatureBaseProperty	m_creatureBaseProperty;

	int			m_remainStatPoint = 0;
	int			m_remainMasteryPoint = 0;

	new void Start () {
		
		m_creatureProperty.init(this, m_creatureBaseProperty);
		m_creatureProperty.Level = 1;

		base.Start();

		SetFollowingCamera(null);
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);

		m_leftJoystick = GameObject.Find("LeftJoystick").GetComponent<Joystick>();
		m_rightJoystick = GameObject.Find("RightJoystick").GetComponent<Joystick>();
	}

	public int RemainStatPoint
	{
		get{return m_remainStatPoint;}
		set{m_remainStatPoint = value;}
	}

	public int RemainMasteryPoint
	{
		get{return m_remainMasteryPoint;}
		set{m_remainMasteryPoint = value;}
	}

	void LevelUp()
	{
		m_remainStatPoint+=1;
		m_remainMasteryPoint+=1;

		GameObject effect = (GameObject)Instantiate(m_prefLevelUpEffect);
		effect.transform.parent = transform;
		effect.transform.localPosition = m_prefLevelUpEffect.transform.position;
		effect.transform.localRotation = m_prefLevelUpEffect.transform.rotation;
		StartCoroutine(UpdateLevelUpEffect(effect));

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
		float step = m_creatureProperty.MoveSpeed;

		if (Application.platform == RuntimePlatform.Android)
		{
			pos.x = m_leftJoystick.position.x*step;
			pos.z = m_leftJoystick.position.y*step;

			m_navAgent.SetDestination(transform.position+pos);

		}
		else
		{
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
	}

	// Update is called once per frame
	new void Update () {
		base.Update();

		UpdateChampMovement();

		if (m_enableAutoTarget)
		{
			if (AutoAttack() == false)
			{
				m_weaponHolder.StopFiring();
			}
		}
		else
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Vector3 pos = Vector3.zero;
				
				pos.x = m_rightJoystick.position.x*10;
				pos.z = m_rightJoystick.position.y*10;

				if (pos.x == 0f && pos.z == 0f)
				{
					m_weaponHolder.StopFiring();
				}
				else
				{
					m_weaponHolder.StartFiring(RotateToTarget(transform.position+pos));
				}

				
			}
			else
			{
				if (Input.GetMouseButton(1) == true)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					pos = ray.origin + (ray.direction* 10f);
					m_weaponHolder.StartFiring(RotateToTarget(pos));
				}
				else
				{
					m_weaponHolder.StopFiring();
				}
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
