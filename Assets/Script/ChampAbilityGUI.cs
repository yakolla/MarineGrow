using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChampAbilityGUI : MonoBehaviour {

	Champ		m_champ;

	YGUISystem.GUIButton[]	m_statButtons = new YGUISystem.GUIButton[3];
	YGUISystem.GUILable		m_remainPointText;
	YGUISystem.GUIPriceButton	m_rollButton;

	int			m_usedCountOfRandomAbilityItem = 0;

	CreatureProperty	m_backup = new CreatureProperty();

	ADMob		m_adMob;

	delegate void OnAbility();
	delegate string OnCompareAbility();
	class Ability
	{
		public float		m_chance;
		public string		m_name;
		public OnCompareAbility m_compare;
		public OnAbility	m_functor;


		public Ability(float chance, string name, OnCompareAbility compare, OnAbility functor)
		{
			m_chance = chance;
			m_name = name;
			m_functor = functor;
			m_compare = compare;
		}
	}

	enum AbilityCategory
	{
		ChampStat,
		Skill,
		Weapon
	}

	Dictionary<AbilityCategory, List<Ability>>	m_abilities = new Dictionary<AbilityCategory, List<Ability>>();
	Ability[]	m_abilitySlots = new Ability[3];

	void Awake()
	{
		List<Ability> champStatsAbili = new List<Ability>();
		List<Ability> skillAbili = new List<Ability>();
		List<Ability> weaponAbili = new List<Ability>();

		champStatsAbili.Add(new Ability(0.3f, "Weapon Damage", 
		()=>{
			m_backup.AlphaPhysicalAttackDamage+=3;
			int backup = m_champ.WeaponHolder.MainWeapon.GetDamage(m_backup);
			
			return m_champ.WeaponHolder.MainWeapon.GetDamage(m_champ.m_creatureProperty) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=3;
			--m_champ.RemainStatPoint;
		}));
		/*
		m_abilities.Add(new Ability(0.3f, "Inc Defence", 
		()=>{
			m_backup.AlphaPhysicalDefencePoint+=3;
			return m_champ.m_creatureProperty.PhysicalDefencePoint + " -> " + "<color=yellow>" + (m_backup.PhysicalDefencePoint) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=3;
			--m_champ.RemainStatPoint;
		}));
		*/
		champStatsAbili.Add(new Ability(0.3f, "Health", 
		()=>{
			m_backup.AlphaMaxHP+=10;
			return m_champ.m_creatureProperty.MaxHP + " -> " + "<color=yellow>" + (m_backup.MaxHP) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMaxHP+=10;
			//m_champ.m_creatureProperty.Heal((int)m_champ.m_creatureProperty.MaxHP);
			--m_champ.RemainStatPoint;
		}));



		champStatsAbili.Add(new Ability(0.3f, "Critical Chance %", 
		()=>{
			m_backup.AlphaCriticalRatio += 0.15f;
			return (m_champ.m_creatureProperty.CriticalRatio*100) + " -> " + "<color=yellow>" + (m_backup.CriticalRatio*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalRatio += 0.15f;
			--m_champ.RemainStatPoint;
		}));

		champStatsAbili.Add(new Ability(0.3f, "Critical Damage %", 
		                            ()=>{
			m_backup.AlphaCriticalDamage += 0.3f;
			return (m_champ.m_creatureProperty.CriticalDamage*100) + " -> " + "<color=yellow>" + (m_backup.CriticalDamage*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalDamage += 0.3f;
			--m_champ.RemainStatPoint;
		}));
/*
		m_abilities.Add(new Ability(0.1f, "Life Per Kill", 
		()=>{
			m_backup.AlphaLifeSteal += 1f;
			return (m_champ.m_creatureProperty.LifeSteal) + " -> " + "<color=yellow>" + (m_backup.LifeSteal) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaLifeSteal += 1f;
			--m_champ.RemainStatPoint;
		}));
*/


		weaponAbili.Add(new Ability(0.01f, "Weapon Levelup", 
		                            ()=>{
			return "";
		},
		()=>{
			m_champ.WeaponHolder.LevelUp();
			--m_champ.RemainStatPoint;
		}));

		weaponAbili.Add(new Ability(0.3f, "Embers On Hit", 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}

			return (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.WeaponHolder.MainWeapon.SetSubWeapon("Pref/Weapon/MineLauncher", 112);
			}

			--m_champ.RemainStatPoint;
		}));

		weaponAbili.Add(new Ability(0.3f, "Splash Range", 
		                            ()=>{

			m_backup.SplashRange+=1;
			
			return (m_champ.m_creatureProperty.SplashRange) + " -> " + "<color=yellow>" + (m_backup.SplashRange) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.SplashRange+=1;
			
			--m_champ.RemainStatPoint;
		}));

		foreach (DamageDesc.BuffType buffType in System.Enum.GetValues(typeof(DamageDesc.BuffType)))
		{
			bool skipBuff = false;
			switch (buffType)
			{
			case DamageDesc.BuffType.Nothing:
			case DamageDesc.BuffType.Count:
			case DamageDesc.BuffType.Combo100:
			case DamageDesc.BuffType.Dash:
			case DamageDesc.BuffType.LevelUp:
			case DamageDesc.BuffType.Stun:
				skipBuff = true;
				break;
			}

			if (skipBuff == true)
				continue;

			string name = "Weapon " + buffType.ToString() + " Effect %";
			DamageDesc.BuffType capturedBuffType = buffType;

			weaponAbili.Add(new Ability(0.3f, name, 
			                            ()=>{
				float oriChance = m_champ.m_creatureProperty.WeaponBuffDescs.chance;
				if (capturedBuffType != m_backup.WeaponBuffDescs.m_buff)
				{
					m_backup.WeaponBuffDescs.chance = 0f;
					oriChance = 0f;
				}
				m_backup.WeaponBuffDescs.chance += 0.1f;
				return (oriChance*100) + " -> " + "<color=yellow>" + (m_backup.WeaponBuffDescs.chance*100) + "</color>";
			},
			()=>{
				if (capturedBuffType != m_champ.m_creatureProperty.WeaponBuffDescs.m_buff)
				{
					m_champ.m_creatureProperty.WeaponBuffDescs.chance = 0f;
				}
				m_champ.m_creatureProperty.WeaponBuffDescs.m_buff = capturedBuffType;
				m_champ.m_creatureProperty.WeaponBuffDescs.chance += 0.1f;
				--m_champ.RemainStatPoint;
			}));
		}

		skillAbili.Add(new Ability(0.3f, "Inc Gain Extra XP %", 
		                                ()=>{
			m_backup.AlphaGainExtraExp += 0.3f;
			return (m_champ.m_creatureProperty.GainExtraExp*100) + " -> " + "<color=yellow>" + (m_backup.GainExtraExp*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaGainExtraExp += 0.3f;
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(0.3f, "Charge Dash Skill", 
		                           ()=>{
			int backup = m_champ.DashSkillStack+10;
			return (m_champ.DashSkillStack) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.DashSkillStack += 10;
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(0.3f, "Charge Combo Skill", 
		                           ()=>{
			int backup = m_champ.ComboSkillStack+1;
			return (m_champ.ComboSkillStack) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.ComboSkillStack += 1;
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(0.3f, "Shield Skill", 
		                           ()=>{
			m_backup.Shield += 30;
			return (m_champ.m_creatureProperty.Shield) + " -> " + "<color=yellow>" + (m_backup.Shield) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.Shield += 30;
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(1f, "Auto Grenade Skill", 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(RefData.Instance.RefItems[105].codeName);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(RefData.Instance.RefItems[105].codeName);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveWeapon(new ItemWeaponData(105, null));
			}
			
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(1f, "Auto Explosion Skill", 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(RefData.Instance.RefItems[129].codeName);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(RefData.Instance.RefItems[129].codeName);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveWeapon(new ItemWeaponData(129, null));
			}
			
			--m_champ.RemainStatPoint;
		}));
		
		m_abilities.Add(AbilityCategory.ChampStat, champStatsAbili);
		m_abilities.Add(AbilityCategory.Skill, skillAbili);
		m_abilities.Add(AbilityCategory.Weapon, weaponAbili);
	}

	void Start () {


		m_statButtons[0] = new YGUISystem.GUIButton(transform.Find("StatButton0").gameObject, ()=>{return true;});
		m_statButtons[1] = new YGUISystem.GUIButton(transform.Find("StatButton1").gameObject, ()=>{return true;});
		m_statButtons[2] = new YGUISystem.GUIButton(transform.Find("StatButton2").gameObject, ()=>{return true;});



		m_remainPointText = new YGUISystem.GUILable(transform.Find("RemainPointText").gameObject);

		m_rollButton = new YGUISystem.GUIPriceButton(transform.Find("RollingButton").gameObject, Const.StartPosYOfPriceButtonImage, ()=>{return true;});
		m_rollButton.Prices = RefData.Instance.RefItems[1101].levelup.conds;


		RandomAbility(null);
	}

	public void StartSpinButton(YGUISystem.GUIButton button)
	{
		button.Button.enabled = false;
		button.Lable.Text.enabled = false;
		button.Button.animator.SetBool("Spin", true);
		button.Button.audio.Play();
	}

	public void StopSpinButton(int slot)
	{
		m_statButtons[slot].Button.enabled = true;
		m_statButtons[slot].Lable.Text.enabled = true;
	}

	void RandomAbility(int[] slots)
	{
		if (slots == null)
		{
			slots = new int[]{0,1,2};
		}

		int selectCount = 0;
		while(selectCount < slots.Length)
		{
			StartSpinButton(m_statButtons[slots[selectCount]]);

			List<Ability> abilis = m_abilities[(AbilityCategory)slots[selectCount]];
			Ability ability = abilis[Random.Range(0, abilis.Count)];
			float ratio = Random.Range(0f, 1f);
			if (ratio < ability.m_chance)
			{
				m_abilitySlots[slots[selectCount]] = ability;
				++selectCount;
			}

		}

	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(true);
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnClickStat(int slot)
	{
		if (m_champ.RemainStatPoint == 0)
			return;

		Ability ability = m_abilitySlots[slot];

		ability.m_functor();
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Ability", ability.m_name, 0);
		RandomAbility(new int[]{slot});
	
	}

	public void OnClickOK()
	{

		for(int i = 0; i < m_statButtons.Length; ++i)
		{
			if (m_statButtons[i].Button.GetComponent<SpinButtonGUI>().IsSpining())
				return;
		}

		gameObject.SetActive(false);
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(false);


	}

	public void OnClickRoll()
	{
		if (true == m_rollButton.TryToPay())
		{
			RandomAbility(null);
			++m_usedCountOfRandomAbilityItem;

			m_rollButton.NormalWorth = 1f+m_usedCountOfRandomAbilityItem;
		}
	}

	void OnGUI()
	{
		if (m_champ == null)
		{
			GameObject obj = GameObject.Find("Champ");
			if (obj == null)
			{
				return;
			}
			
			m_champ = obj.GetComponent<Champ>();
			return;
		}

		m_champ.m_creatureProperty.CopyTo(m_backup);

		int statSlot = 0;
		foreach(YGUISystem.GUIButton button in m_statButtons)
		{
			button.Lable.Text.text = m_abilitySlots[statSlot].m_name + "\n" + m_abilitySlots[statSlot].m_compare();
			++statSlot;
		}

		m_remainPointText.Text.text = m_champ.RemainStatPoint.ToString();


		m_rollButton.Update();

	}

}
