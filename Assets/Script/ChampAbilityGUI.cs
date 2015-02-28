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

	delegate void OnAbility();
	class Ability
	{
		public float		m_ratio;
		public string		m_name;
		public OnAbility	m_functor;


		public Ability(float ratio, string name, OnAbility functor)
		{
			m_ratio = ratio;
			m_name = name;
			m_functor = functor;
		}
	}

	List<Ability>	m_abilities = new List<Ability>();
	int[]	m_randomAbility = new int[3];

	void Awake()
	{
		m_abilities.Add(new Ability(0.3f, "Inc Attack", ()=>{
			m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=1;
			--m_champ.RemainStatPoint;
		}));
		
		m_abilities.Add(new Ability(0.3f, "Inc Defence", ()=>{
			m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=1;
			--m_champ.RemainStatPoint;
		}));
		
		m_abilities.Add(new Ability(0.3f, "Inc MaxHP", ()=>{
			m_champ.m_creatureProperty.AlphaMaxHP+=1;
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.01f, "Weapon Evolution", ()=>{
			m_champ.WeaponHolder.Evolution();
			--m_champ.RemainStatPoint;
		}));

		m_abilities.Add(new Ability(0.05f, "Weapon Levelup", ()=>{
			m_champ.WeaponHolder.Evolution();
			--m_champ.RemainStatPoint;
		}));
	}

	void Start () {

		m_champ = transform.parent.gameObject.GetComponent<Champ>();
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);

	}

	void RandomAbility()
	{
		List<int> indexs = new List<int>();
		for(int i = 0; i < m_abilities.Count; ++i)
		{
			indexs.Add(i);
		}

		int selectCount = 0;
		while(selectCount < m_randomAbility.Length)
		{
			int rid = Random.Range(0, indexs.Count);
			float ratio = Random.Range(0f, 1f);
			if (ratio > m_abilities[indexs[rid]].m_ratio)
				continue;
			Debug.Log(ratio);

			m_randomAbility[selectCount] = indexs[rid];
			indexs.RemoveAt(rid);
			++selectCount;
		}

	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
		RandomAbility();
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;

		m_statusWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampLevelUp, m_statusWindowRect, DisplayStatusWindow, "");	
	}

	void Update()
	{
		TimeEffector.Instance.StopTime();
	}

	Vector2[] masterySrollPosition = new Vector2[MasteryTypes];
	void displayMastery(Rect rect, Ability ability)
	{
		int size = (int)rect.width/MasteryColumns;
		GUI.BeginGroup(rect);
		if (GUI.Button(new Rect(0, 0, rect.width, rect.height), ability.m_name) && m_champ.RemainStatPoint > 0)
		{
			ability.m_functor();
		}

		GUI.EndGroup();
	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;

		if (GUI.Button(new Rect(Screen.width/2-size/2, Screen.height-size, size, size), "OK"))
		{
			this.gameObject.SetActive(false);
			return;
		}

		GUI.Label(new Rect(0, startY+(size*0), size*2, size), "StatPoint:");
		GUI.Label(new Rect(size*2, startY+(size*0), size, size), m_champ.RemainStatPoint.ToString());

		GUI.Label(new Rect(size*3, startY+(size*0), size*2, size), "MasteryPoint:");
		GUI.Label(new Rect(size*5, startY+(size*0), size, size), m_champ.RemainMasteryPoint.ToString());

		int masteryWidth = Screen.width/3;
		displayMastery(new Rect(0, size, masteryWidth, Screen.height-size*2), 		              
		               m_abilities[m_randomAbility[0]]
		);

		displayMastery(new Rect(masteryWidth, size, masteryWidth, Screen.height-size*2), 
		               m_abilities[m_randomAbility[1]]
		);

		displayMastery(new Rect(masteryWidth*2, size, masteryWidth, Screen.height-size*2), 
		               m_abilities[m_randomAbility[2]]
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
