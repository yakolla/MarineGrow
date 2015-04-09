using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChampAbilityGUI : MonoBehaviour {

	Champ		m_champ;
	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;
	const 	int MasteryTypes = 3;
	const	int	MasteryColumns = 4;
	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);
	int		m_fontSize = (int)(Screen.width*(1/50f));
	int			m_usedCountOfRandomAbilityItem = 0;
	CreatureProperty	m_backup = new CreatureProperty();

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
			return m_champ.m_creatureProperty.PhysicalAttackDamage + " -> " + "<color=yellow>" + (m_backup.PhysicalAttackDamage) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=3;
			--m_champ.RemainStatPoint;
		}));
		
		m_abilities.Add(new Ability(0.3f, "Inc Defence", 
		()=>{
			m_backup.AlphaPhysicalDefencePoint+=3;
			return m_champ.m_creatureProperty.PhysicalDefencePoint + " -> " + "<color=yellow>" + (m_backup.PhysicalDefencePoint) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=3;
			--m_champ.RemainStatPoint;
		}));
		
		m_abilities.Add(new Ability(0.3f, "Inc Health", 
		()=>{
			m_backup.AlphaMaxHP+=10;
			return m_champ.m_creatureProperty.MaxHP + " -> " + "<color=yellow>" + (m_backup.MaxHP) + "</color>";
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
			return (m_champ.m_creatureProperty.CriticalRatio*100) + " -> " + "<color=yellow>" + (m_backup.CriticalRatio*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalRatio += 0.03f;
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.3f, "Inc Critical Damage %", 
		                            ()=>{
			m_backup.AlphaCriticalDamage += 0.3f;
			return (m_champ.m_creatureProperty.CriticalDamage*100) + " -> " + "<color=yellow>" + (m_backup.CriticalDamage*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaCriticalDamage += 0.3f;
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.1f, "Life Per Kill", 
		()=>{
			m_backup.AlphaLifeSteal += 1f;
			return (m_champ.m_creatureProperty.LifeSteal) + " -> " + "<color=yellow>" + (m_backup.LifeSteal) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaLifeSteal += 1f;
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.3f, "Inc Gain Extra Exp %", 
		                            ()=>{
			m_backup.AlphaGainExtraExp += 0.3f;
			return (m_champ.m_creatureProperty.GainExtraExp*100) + " -> " + "<color=yellow>" + (m_backup.GainExtraExp*100) + "</color>";
		},
		()=>{
			m_champ.m_creatureProperty.AlphaGainExtraExp += 0.3f;
			--m_champ.RemainStatPoint;
		}));
	}

	void Start () {

		m_champ = transform.parent.gameObject.GetComponent<Champ>();
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
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

	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;

		m_statusWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampLevelUp, m_statusWindowRect, DisplayStatusWindow, "");	
	}

	void Update()
	{
		TimeEffector.Instance.StopTime();
	}

	Vector2[] masterySrollPosition = new Vector2[MasteryTypes];
	void displayMastery(int slot, Rect rect, Ability ability)
	{
		int size = (int)rect.width/MasteryColumns;
		GUI.BeginGroup(rect);
		if (GUI.Button(new Rect(0, 0, rect.width, rect.height), ability.m_name + "\n" + ability.m_compare()) && m_champ.RemainStatPoint > 0)
		{
			ability.m_functor();
			GPlusPlatform.Instance.AnalyticsTrackEvent("InGame", "Ability", ability.m_name, 0);
			RandomAbility(new int[]{slot});
		}

		GUI.EndGroup();
	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startX = 0;
		int startY = 0;
		int size = (int)m_height;

		if (GUI.Button(new Rect(Screen.width/2-(size*4)/2, Screen.height-size*2, size*2, size*2), "OK"))
		{
			this.gameObject.SetActive(false);
			return;
		}

		if (m_champ.RemainStatPoint > 0)
		{
			Const.makeItemButton(m_guiSkin, m_fontSize, Screen.width/2-size/2+size, Screen.height-size*2, size, size, RefData.Instance.RefItems[1002].levelup, 1f+m_usedCountOfRandomAbilityItem, "Roll", ()=>{
				RandomAbility(null);
				++m_usedCountOfRandomAbilityItem;
			});
		}


		GUI.Label(new Rect(0, startY+(size*0), size*2, size), "StatPoint:");
		GUI.Label(new Rect(size*2, startY+(size*0), size, size), m_champ.RemainStatPoint.ToString());

		GUI.Label(new Rect(size*3, startY+(size*0), size*2, size), "MasteryPoint:");

		m_champ.m_creatureProperty.CopyTo(m_backup);

		int masteryWidth = Screen.width/3;
		displayMastery(0, new Rect(0, size, masteryWidth, Screen.height-size*3), 		              
		               m_abilities[m_abilitySlots[0]]
		);

		displayMastery(1, new Rect(masteryWidth, size, masteryWidth, Screen.height-size*3), 
		               m_abilities[m_abilitySlots[1]]
		);

		displayMastery(2, new Rect(masteryWidth*2, size, masteryWidth, Screen.height-size*3), 
		               m_abilities[m_abilitySlots[2]]
		);


	}
	void drawGuage(Rect size, float ratio, string lable, Texture guage)
	{
		GUI.DrawTextureWithTexCoords(new Rect(size.x, size.y, size.width*ratio, size.height), guage, new Rect(0f, 0f, ratio, 1f));
		GUIStyle style = new GUIStyle();
		style.fontSize = 10;
		style.normal.textColor = Color.grey;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(size.x, size.y, size.width, size.height), lable, style);
		style.normal.textColor = Color.black;
		GUI.Label(new Rect(size.x+1, size.y+1, size.width, size.height), lable, style);
	}
	//Setting up the Inventory window
	void DisplayGuageWindow(int windowID)
	{
		float hp = m_champ.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, 100, 15), hp, lable, Resources.Load<Texture>("Sprites/HP Guage")); 

	}


}
