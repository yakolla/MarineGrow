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
		champStatsAbili.Add(new Ability(0.3f, "Health Max Up",
		()=>{
			m_backup.AlphaMaxHP+=50;
			return m_champ.m_creatureProperty.MaxHP + " -> " + "<color=yellow>" + (m_backup.MaxHP) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMaxHP+=50;
			//m_champ.m_creatureProperty.Heal((int)m_champ.m_creatureProperty.MaxHP);
			--m_champ.RemainStatPoint;
		}));

		champStatsAbili.Add(new Ability(0.3f, "SP Max Up", 
		                                ()=>{
			m_backup.AlphaMaxSP+=30;
			return m_champ.m_creatureProperty.MaxSP + " -> " + "<color=yellow>" + (m_backup.MaxSP) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMaxSP+=30;
			--m_champ.RemainStatPoint;
		}));

		champStatsAbili.Add(new Ability(0.3f, "Move Speed Up", 
		                                ()=>{
			m_backup.AlphaMoveSpeed+=1;
			return m_champ.m_creatureProperty.AlphaMoveSpeed + " -> " + "<color=yellow>" + (m_backup.AlphaMoveSpeed) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMoveSpeed+=1;
			--m_champ.RemainStatPoint;
		}));

		champStatsAbili.Add(new Ability(0.3f, "SP Recovery Up", 
		                                ()=>{
			m_backup.AlphaSPRecoveryPerSec+=1;
			return m_champ.m_creatureProperty.SPRecoveryPerSec + " -> " + "<color=yellow>" + (m_backup.SPRecoveryPerSec) + "/sec</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaSPRecoveryPerSec+=1;
			--m_champ.RemainStatPoint;
		}));

		champStatsAbili.Add(new Ability(0.3f, "Critical Suites",
		()=>{
			m_backup.AlphaCriticalRatio += 0.15f;
			m_backup.AlphaCriticalDamage += 0.3f;
			return "Chance:"+(m_champ.m_creatureProperty.CriticalChance*100) + " -> " + "<color=yellow>" + (m_backup.CriticalChance*100) + "%</color>" + "\n" +
				"Damage:"+(m_champ.m_creatureProperty.CriticalDamage*100) + " -> " + "<color=yellow>" + (m_backup.CriticalDamage*100) + "%</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalRatio += 0.15f;
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


		weaponAbili.Add(new Ability(0.01f, "Weapon Lv Up", 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon;
			int backup = weapon.Level+1;
			int ori = weapon.Level;
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.WeaponHolder.LevelUp();
			--m_champ.RemainStatPoint;
		}));

		weaponAbili.Add(new Ability(0.3f, "Embers Skill", 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}

			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>" + "\n" +
				"SP:" + Weapon.GetSP(RefData.Instance.RefItems[132], ori) + " -> " + "<color=yellow>" +Weapon.GetSP(RefData.Instance.RefItems[132], backup)+ "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.MainWeapon.GetSubWeapon();
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.SetSubWeapon(m_champ.WeaponHolder.MainWeapon, new ItemWeaponData(132, null));
			}

			--m_champ.RemainStatPoint;
		}));

		weaponAbili.Add(new Ability(0.3f, "Splash Range", 
		                            ()=>{

			m_backup.SplashRange+=1;
			
			return (m_champ.m_creatureProperty.SplashRange) + " -> " + "<color=yellow>" + (m_backup.SplashRange) + "m</color>";
		},
		()=>{
			m_champ.m_creatureProperty.SplashRange+=1;
			
			--m_champ.RemainStatPoint;
		}));

		weaponAbili.Add(new Ability(0.3f, "Callable Followers", 
		                            ()=>{
			
			m_backup.CallableFollowers+=1;
			
			return (m_champ.m_creatureProperty.CallableFollowers) + " -> " + "<color=yellow>" + (m_backup.CallableFollowers) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.CallableFollowers+=1;
			
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

			string name = buffType.ToString() + " Chance";
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
				return (oriChance*100) + " -> " + "<color=yellow>" + (m_backup.WeaponBuffDescs.chance*100) + "%</color>";
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

		skillAbili.Add(new Ability(0.3f, "Gain Extra XP", 
		                                ()=>{
			m_backup.AlphaGainExtraExp += 0.5f;
			return (m_champ.m_creatureProperty.GainExtraExp*100) + " -> " + "<color=yellow>" + (m_backup.GainExtraExp*100) + "%</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaGainExtraExp += 0.5f;
			--m_champ.RemainStatPoint;
		}));
		/*
		skillAbili.Add(new Ability(0.3f, "Charge to Dash Skill", 
		                           ()=>{
			int backup = m_champ.NuclearSkillStack+10;
			return (m_champ.NuclearSkillStack) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.NuclearSkillStack += 10;
			--m_champ.RemainStatPoint;
		}));
*/
		skillAbili.Add(new Ability(0.3f, "Charge to Combo Skill", 
		                           ()=>{
			int backup = m_champ.ComboSkillStack+3;
			return (m_champ.ComboSkillStack) + " -> " + "<color=yellow>" + (backup) + "</color>";
		},
		()=>{
			m_champ.ComboSkillStack += 3;
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(0.3f, "Grenade Skill", 
		                            ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(131);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>" + "\n" +
				"SP:" + Weapon.GetSP(RefData.Instance.RefItems[131], ori) + " -> " + "<color=yellow>" +Weapon.GetSP(RefData.Instance.RefItems[131], backup)+ "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(131);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveWeapon(new ItemWeaponData(131, null));
			}
			
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(0.3f, "Explosion Skill", 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(129);
			int backup = 1;
			int ori = 0;
			if (weapon != null)
			{
				backup = weapon.Level+1;
				ori = weapon.Level;
			}
			
			return "Lv:" + (ori) + " -> " + "<color=yellow>" + (backup) + "</color>" + "\n" +
				"SP:" + Weapon.GetSP(RefData.Instance.RefItems[129], ori) + " -> " + "<color=yellow>" +Weapon.GetSP(RefData.Instance.RefItems[129], backup)+ "</color>";
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(129);
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

		skillAbili.Add(new Ability(0.3f, "Charge to Nuclear Skill", 
		                           ()=>{
			return (m_champ.NuclearSkillStack) + " -> " + "<color=yellow>" + (m_champ.NuclearSkillStack+1) + "</color>" + "\n" +
					"SP:" + Weapon.GetSP(RefData.Instance.RefItems[133], 1);
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetActiveWeapon(133);
			if (weapon != null)
			{
				//weapon.LevelUp();
			}
			else
			{
				m_champ.EquipActiveWeapon(new ItemWeaponData(133, null));
			}
			++m_champ.NuclearSkillStack;
			--m_champ.RemainStatPoint;
		}));

		skillAbili.Add(new Ability(0.3f, "Charge to Shield", 
		                           ()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(130);
			m_backup.Shield += 10;
			return (m_champ.m_creatureProperty.Shield) + " -> " + "<color=yellow>" + (m_backup.Shield) + "</color>" + "\n" +
				"SP:" + Weapon.GetSP(RefData.Instance.RefItems[130], 1);
		},
		()=>{
			Weapon weapon = m_champ.WeaponHolder.GetPassiveWeapon(130);
			if (weapon != null)
			{
				weapon.LevelUp();
			}
			else
			{
				m_champ.EquipPassiveWeapon(new ItemWeaponData(130, null));
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

		m_rollButton = new YGUISystem.GUIPriceButton(transform.Find("RollingButton").gameObject, Const.StartPosYOfPriceButtonImage, ()=>{return m_champ.RemainStatPoint > 0;});
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

			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Ability", "Roll"+m_usedCountOfRandomAbilityItem, 0);
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
