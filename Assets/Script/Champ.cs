using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

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

	[SerializeField]
	int			m_remainStatPoint = 0;

	int			m_comboKills;

	int			m_comboSkillStacks = 0;

	int			m_level = 1;

	Animator	m_bloodWarningAnimator;

	[SerializeField]
	GUISkin		m_guiSkin = null;

	new void Start () {

		base.Start();

		SetFollowingCamera(null);
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetMainTarget(gameObject);

		m_leftJoystick = GameObject.Find("HudGUI/LeftJoystick").GetComponent<Joystick>();
		m_rightJoystick = GameObject.Find("HudGUI/RightJoystick").GetComponent<Joystick>();

		m_bloodWarningAnimator = GameObject.Find("HudGUI/Blood Warning").GetComponent<Animator>();
	}

	override public void Init()
	{
		base.Init();

		m_level = Warehouse.Instance.champAbility.m_level;
		m_creatureProperty.init(this, m_creatureBaseProperty, m_level);
		m_remainStatPoint = 0;
		m_comboKills = 0;
		m_comboSkillStacks = 0;
		m_startChargeTime = 0f;

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

		GameObject effect = (GameObject)Instantiate(m_prefLevelUpEffect);
		effect.transform.parent = transform;
		effect.transform.localPosition = m_prefLevelUpEffect.transform.position;
		effect.transform.localRotation = m_prefLevelUpEffect.transform.rotation;
		StartCoroutine(UpdateLevelUpEffect(effect));

		ApplyBuff(null, DamageDesc.BuffType.LevelUp, 10f, null);

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

		GUIStyle levelupStyle = m_guiSkin.GetStyle("DashSkill");
		levelupStyle.fontSize = Const.m_fontSize;		

		Rect skillButton = new Rect(Screen.width-Const.m_slotWidth, Const.m_slotHeight*2, Const.m_slotHeight, Const.m_slotHeight);
		
		Const.GuiButtonMultitouchable(skillButton, "", levelupStyle, ()=>{

			if (pos != Vector3.zero)
			{
				DamageDesc desc  = new DamageDesc(0, DamageDesc.Type.Normal, DamageDesc.BuffType.Dash, Resources.Load<GameObject>("Pref/ef_dash"));
				desc.Dir = pos.normalized;
				ApplyBuff(null, DamageDesc.BuffType.Dash, 0.5f, desc);
			}

		});
	}

	public int ComboKills
	{
		get {return m_comboKills;}
		set {m_comboKills = value;}
	}



	public int ComboSkillStack
	{
		get {return m_comboSkillStacks;}
		set {m_comboSkillStacks = value;}
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

			GPlusPlatform.Instance.OpenGame(Warehouse.Instance.FileName, OnSavedGameOpenedForSaving);
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
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.Up);
			break;
		case DamageDesc.BuffType.LevelUp:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.Up);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "ChampLevelUp", "ChampLV:" + m_creatureProperty.Level, 0);
			break;
		case DamageDesc.BuffType.Combo100:
			DamageText(type.ToString(), Color.cyan, DamageNumberSprite.MovementType.Up);
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Combo", "Combo100", 0);
			break;		
		}
		return true;
	}




	void OnSavedGameOpenedForSaving(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			System.TimeSpan totalPlayingTime = game.TotalTimePlayed;

			GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, Const.getScreenshot(), OnSavedGameWritten);

			GPlusPlatform.Instance.ReportScore(Const.LEAD_COMBO_MAX_KILLS, Warehouse.Instance.Stats.m_comboKills, (bool success) => {
				// handle success or failure
			});
		} else {
			// handle error
		}

	}

	void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			// handle reading or writing of saved game.
		} else {
			// handle error
		}

		LoadTitleScene(0);
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
