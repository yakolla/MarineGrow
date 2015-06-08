using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	int			m_nuclearSkillStacks = 0;


	Animator	m_bloodWarningAnimator;

	Vector3		m_moveDir;



	new void Start () {

		base.Start();

		ApplyGameOptions();

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

		m_creatureProperty.init(this, m_creatureBaseProperty, 1);
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

	public int NuclearSkillStack
	{
		get {return m_nuclearSkillStacks;}
		set {m_nuclearSkillStacks = value;}
	}

	public void ApplyGameOptions()
	{
		audio.ignoreListenerVolume = false;
		audio.volume = Warehouse.Instance.GameOptions.m_bgmVolume;
		audio.ignoreListenerVolume = true;

		AudioListener.volume = Warehouse.Instance.GameOptions.m_sfxVolume;
		m_enableAutoTarget = Warehouse.Instance.GameOptions.m_autoTarget;
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
		exp = (int)(exp+exp*m_creatureProperty.GainExtraExp);
		Warehouse.Instance.NewGameStats.GainedXP += exp;
		m_creatureProperty.giveExp(exp);
	}

	override public void TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		base.TakeDamage(offender, damageDesc);
		m_bloodWarningAnimator.SetTrigger("Warning");
	}



	override public void Death()
	{
		base.Death();

		Warehouse.Instance.NewGameStats.SurvivalTime = Warehouse.Instance.PlayTime;
		
		GPlusPlatform.Instance.ReportScore(Const.LEADERBOARD_GAINED_GOLD, Warehouse.Instance.NewGameStats.GainedGold, (bool success) => {
			// handle success or failure
		});
		
		GPlusPlatform.Instance.ReportScore(Const.LEADERBOARD_GAINED_XP, Warehouse.Instance.NewGameStats.GainedXP, (bool success) => {
			// handle success or failure
		});
		
		System.TimeSpan totalPlayingTime = new System.TimeSpan((long)(System.TimeSpan.TicksPerSecond*Warehouse.Instance.NewGameStats.SurvivalTime));
		GPlusPlatform.Instance.ReportScore(Const.LEADERBOARD_SURVIVAL_TIME,  (long)(totalPlayingTime.TotalMilliseconds), (bool success) => {
			// handle success or failure
		});
		
		GPlusPlatform.Instance.ReportScore(Const.LEADERBOARD_KILLED_MOBS, Warehouse.Instance.NewGameStats.KilledMobs, (bool success) => {
			// handle success or failure
		});
		
		Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
			if (status == SavedGameRequestStatus.Success) {
				// handle reading or writing of saved game.
			} else {
				// handle error
			}
		});

		Const.GetSpawn().StartCoroutine(ShowGameOverGUI());
	}

	IEnumerator	ShowGameOverGUI()
	{
		yield return new WaitForSeconds(2f);
		
		GameObject.Find("HudGUI/GameOverGUI").transform.Find("Panel").gameObject.SetActive(true);
	}


	override public bool ApplyBuff(Creature offender, DamageDesc.BuffType type, float time, DamageDesc damageDesc)
	{
		if (false == base.ApplyBuff(offender, type, time, damageDesc))
			return false;

		switch(type)
		{		
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
