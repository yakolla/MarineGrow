using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChampAbilityGUI : MonoBehaviour {

	Champ		m_champ;

	YGUISystem.GUIButton[]	m_statButtons = new YGUISystem.GUIButton[3];
	YGUISystem.GUIText		m_remainPointText;
	YGUISystem.GUIPriceButton	m_rollButton;

	int			m_usedCountOfRandomAbilityItem = 0;

	CreatureProperty	m_backup = new CreatureProperty();

	ADMob		m_adMob;

	delegate void OnAbility();
	delegate string OnCompareAbility();
	class Ability
	{
		public float		m_ratio;
		public string		m_name;
		public OnCompareAbility m_compare;
		public OnAbility	m_functor;


		public Ability(float ratio, string name, OnCompareAbility compare, OnAbility functor)
		{
			m_ratio = ratio;
			m_name = name;
			m_functor = functor;
			m_compare = compare;
		}
	}

	List<Ability>	m_abilities = new List<Ability>();
	int[]	m_abilitySlots = new int[3];

	void Awake()
	{
		m_abilities.Add(new Ability(0.3f, "Inc Strength", 
		()=>{
			m_backup.AlphaPhysicalAttackDamage+=3;
			return m_champ.m_creatureProperty.PhysicalAttackDamage + " -> " + "<color=green>" + (m_backup.PhysicalAttackDamage) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=3;
			--m_champ.RemainStatPoint;
		}));
		/*
		m_abilities.Add(new Ability(0.3f, "Inc Defence", 
		()=>{
			m_backup.AlphaPhysicalDefencePoint+=3;
			return m_champ.m_creatureProperty.PhysicalDefencePoint + " -> " + "<color=green>" + (m_backup.PhysicalDefencePoint) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=3;
			--m_champ.RemainStatPoint;
		}));
		*/
		m_abilities.Add(new Ability(0.3f, "Inc Health", 
		()=>{
			m_backup.AlphaMaxHP+=10;
			return m_champ.m_creatureProperty.MaxHP + " -> " + "<color=green>" + (m_backup.MaxHP) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaMaxHP+=10;
			m_champ.m_creatureProperty.Heal((int)m_champ.m_creatureProperty.MaxHP);
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.01f, "Weapon Levelup", 
		                            ()=>{
			return "";
		},
		()=>{
			m_champ.WeaponHolder.LevelUp();
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.3f, "Inc Critical Chance %", 
		()=>{
			m_backup.AlphaCriticalRatio += 0.03f;
			return (m_champ.m_creatureProperty.CriticalRatio*100) + " -> " + "<color=green>" + (m_backup.CriticalRatio*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalRatio += 0.03f;
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.3f, "Inc Critical Damage %", 
		                            ()=>{
			m_backup.AlphaCriticalDamage += 0.3f;
			return (m_champ.m_creatureProperty.CriticalDamage*100) + " -> " + "<color=green>" + (m_backup.CriticalDamage*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalDamage += 0.3f;
			--m_champ.RemainStatPoint;
		}));
/*
		m_abilities.Add(new Ability(0.1f, "Life Per Kill", 
		()=>{
			m_backup.AlphaLifeSteal += 1f;
			return (m_champ.m_creatureProperty.LifeSteal) + " -> " + "<color=green>" + (m_backup.LifeSteal) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaLifeSteal += 1f;
			--m_champ.RemainStatPoint;
		}));
*/
		m_abilities.Add(new Ability(0.3f, "Inc Gain Extra XP %", 
		                            ()=>{
			m_backup.AlphaGainExtraExp += 0.3f;
			return (m_champ.m_creatureProperty.GainExtraExp*100) + " -> " + "<color=green>" + (m_backup.GainExtraExp*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaGainExtraExp += 0.3f;
			--m_champ.RemainStatPoint;
		}));
	}

	void Start () {


		m_statButtons[0] = new YGUISystem.GUIButton(transform.Find("StatButton0").gameObject, ()=>{return true;});
		m_statButtons[1] = new YGUISystem.GUIButton(transform.Find("StatButton1").gameObject, ()=>{return true;});
		m_statButtons[2] = new YGUISystem.GUIButton(transform.Find("StatButton2").gameObject, ()=>{return true;});

		m_remainPointText = new YGUISystem.GUIText(transform.Find("RemainPointText").gameObject);

		m_rollButton = new YGUISystem.GUIPriceButton(transform.Find("RollingButton").gameObject, Const.StartPosYOfPriceButtonImage, ()=>{return true;});
		m_rollButton.Prices = RefData.Instance.RefItems[1101].levelup.conds;


		RandomAbility(null);
	}

	void RandomAbility(int[] slots)
	{
		if (slots == null)
		{
			slots = new int[]{0,1,2};
		}

		foreach(int slot in slots)
		{
			m_abilitySlots[slot] = -1;
		}

		List<int> indexs = new List<int>();
		for(int i = 0; i < m_abilities.Count; ++i)
		{
			bool skip = false;
			foreach(int slot in m_abilitySlots)
			{
				if (slot == i)
				{
					skip = true;
					break;
				}
			}

			if (skip == true)
				continue;

			indexs.Add(i);
		}

		int selectCount = 0;
		while(selectCount < slots.Length)
		{
			int rid = Random.Range(0, indexs.Count);
			float ratio = Random.Range(0f, 1f);
			if (ratio > m_abilities[indexs[rid]].m_ratio)
				continue;

			m_abilitySlots[slots[selectCount]] = indexs[rid];
			indexs.RemoveAt(rid);
			++selectCount;
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

		Ability ability = m_abilities[m_abilitySlots[slot]];

		ability.m_functor();
		GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Ability", ability.m_name, 0);
		RandomAbility(new int[]{slot});
	
	}

	public void OnClickOK()
	{
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
			button.Text.Lable = m_abilities[m_abilitySlots[statSlot]].m_name + "\n" + m_abilities[m_abilitySlots[statSlot]].m_compare();
			++statSlot;
		}

		m_remainPointText.Lable = m_champ.RemainStatPoint.ToString();


		m_rollButton.Update();

	}

	void Update()
	{
		TimeEffector.Instance.StopTime();
	}
}
