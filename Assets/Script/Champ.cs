using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class Champ : Creature {


	Joypad		m_leftJoypad;
	Joypad		m_rightJoypad;

	[SerializeField]
	bool	m_enableAutoTarget = true;

	[SerializeField]
	Vector3	m_cameraOffset;

	[SerializeField]
	RefCreatureBaseProperty	m_creatureBaseProperty;

	[SerializeField]
	int			m_remainStatPoint = 0;

	[SerializeField]
	int			m_comboKills;


	int			m_comboSkillStacks = 0;
	int			m_dashSkillStacks = 0;

	int			m_level = 1;

	Animator	m_bloodWarningAnimator;

	Vector3		m_moveDir;


	new void Start () {

		base.Start();

		SetFollowingCamera(null);
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);

		m_leftJoypad = GameObject.Find("HudGUI/Joypad/LeftJoypad").GetComponent<Joypad>();
		m_rightJoypad = GameObject.Find("HudGUI/Joypad/RightJoypad").GetComponent<Joypad>();

		m_bloodWarningAnimator = GameObject.Find("HudGUI/Blood Warning").GetComponent<Animator>();
	}

	override public void Init()
	{
		base.Init();

		m_level = Warehouse.Instance.champAbility.m_level;
		m_creatureProperty.init(this, m_creatureBaseProperty, m_level);
		m_comboKills = 0;
		m_comboSkillStacks = 0;

	}

	public int RemainStatPoint
	{
		get{return m_remainStatPoint;}
		set{m_remainStatPoint = value;}
	}

	void LevelUp()
	{
		m_remainStatPoint+=1;
		++m_level;

		ApplyBuff(null, DamageDesc.BuffType.LevelUp, 10f, null);

	}


	void UpdateChampMovement()
	{
		m_moveDir = Vector3.zero;
		if (HasCrowdControl())
			return;

		Vector3 pos = Vector3.zero;
		float step = m_creatureProperty.MoveSpeed;

		if (Application.platform == RuntimePlatform.Android)
		{
			if (m_leftJoypad.Dragging)
			{
				pos.x = m_leftJoypad.Position.x*step;
				pos.z = m_leftJoypad.Position.y*step;
				
				m_navAgent.SetDestination(transform.position+pos);
			}

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

			if (m_leftJoypad.Dragging)
			{
				pos.x = m_leftJoypad.Position.x*step;
				pos.z = m_leftJoypad.Position.y*step;
				
				m_navAgent.SetDestination(transform.position+pos);
			}
		}

		m_moveDir = pos.normalized;
	}

	public int ComboKills
	{
		get {return m_comboKills;}
		set {m_comboKills = value;}
	}

	public Vector3 MoveDir
	{
		get {return m_moveDir;}
	}

	public int ComboSkillStack
	{
		get {return m_comboSkillStacks;}
		set {m_comboSkillStacks = value;}
	}

	public int DashSkillStack
	{
		get {return m_dashSkillStacks;}
		set {m_dashSkillStacks = value;}
	}

	void OnGUI()
	{
		UpdateChampMovement();
	}

	// Update is called once per frame
	new void Update () {
		base.Update();



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
				if (m_rightJoypad.Dragging)
				{
					Vector3 pos = Vector3.zero;
					pos.x = m_rightJoypad.Position.x*10;
					pos.z = m_rightJoypad.Position.y*10;
					m_weaponHolder.StartFiring(RotateToTarget(transform.position+pos));
				}
				else
				{
					m_weaponHolder.StopFiring();
				}
			}
			else
			{
				if (m_rightJoypad.Dragging)
				{
					Vector3 pos = Vector3.zero;
					pos.x = m_rightJoypad.Position.x*10;
					pos.z = m_rightJoypad.Position.y*10;
					m_weaponHolder.StartFiring(RotateToTarget(transform.position+pos));
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

	override public void GiveExp(int exp)
	{
		m_creatureProperty.giveExp((int)(exp+exp*m_creatureProperty.GainExtraExp));
	}

	override public void TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		base.TakeDamage(offender, damageDesc);
		ComboKills = 0;
		m_bloodWarningAnimator.SetTrigger("Warning");
	}

	override public void Death()
	{
		base.Death();

		Warehouse.Instance.champAbility.m_level = 1;
		Warehouse.Instance.champAbility.m_abilityPoint = (int)(m_level*0.5f);

		if (Application.platform == RuntimePlatform.Android)
		{

			Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
				if (status == SavedGameRequestStatus.Success) {
					// handle reading or writing of saved game.
				} else {
					// handle error
				}
				
				GPlusPlatform.Instance.ReportScore(Const.LEAD_COMBO_MAX_KILLS, Warehouse.Instance.Stats.m_comboKills, (bool success) => {
					// handle success or failure
				});
				
				LoadTitleScene(0);
			});
		}
		else
		{
			LoadTitleScene(2);

		}

	}

	void LoadTitleScene(float delay)
	{
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowInterstitial();
		GameObject.Find("Dungeon").GetComponent<Dungeon>().DelayLoadLevel(delay);
	}

	override public bool ApplyBuff(Creature offender, DamageDesc.BuffType type, float time, DamageDesc damageDesc)
	{
		if (false == base.ApplyBuff(offender, type, time, damageDesc))
			return false;

		switch(type)
		{
		case DamageDesc.BuffType.Airborne:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			break;
		case DamageDesc.BuffType.LevelUp:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "ChampLevelUp", "ChampLV:" + m_creatureProperty.Level, 0);
			break;
		case DamageDesc.BuffType.Combo100:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.RisingUp);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Combo", "Combo100", 0);
			break;		
		}
		return true;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("ItemBox") == 0)
		{
			if (3f > Vector3.Distance(transform.position, other.transform.position))
			{
				ItemBox itemBox = other.gameObject.GetComponent<ItemBox>();
				itemBox.StartPickupEffect(this);
			}
		};

	}
}
