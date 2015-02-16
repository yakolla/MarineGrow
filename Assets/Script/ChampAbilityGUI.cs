using UnityEngine;
using System.Collections;

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

	void Start () {

		m_champ = transform.parent.gameObject.GetComponent<Champ>();
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
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

		m_statusWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampLevelUp, m_statusWindowRect, DisplayStatusWindow, "");	
	}

	void Update()
	{
		TimeEffector.Instance.StopTime();
	}

	Vector2[] masterySrollPosition = new Vector2[MasteryTypes];
	delegate void OnChampStat();
	void displayMastery(Rect rect, Texture2D icon, float statPoint, ref Vector2 scrollPosition, OnChampStat onChampStat)
	{
		int size = (int)rect.width/MasteryColumns;
		GUI.BeginGroup(rect);
		GUI.Label(new Rect(0, 0, size, size), icon);
		GUI.Label(new Rect(size, 0, size, size), statPoint.ToString());
		if (GUI.Button(new Rect(size*3, 0, size, size), "+") && m_champ.RemainStatPoint > 0)
		{
			onChampStat();
		}

		int padding = size/MasteryColumns;

		switch (Application.platform)
		{
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
			scrollPosition = GUI.BeginScrollView(new Rect(0, size+padding, size*MasteryColumns, size*4), scrollPosition, new Rect(0, size, size*MasteryColumns-padding*2, size*8));
			break;
		default:
			scrollPosition = GUI.BeginScrollView(new Rect(0, size+padding, size*MasteryColumns, size*4), scrollPosition, new Rect(0, size+padding, size*MasteryColumns-padding*2, size*8),GUIStyle.none,GUIStyle.none);
			break;
		}

		GUIStyle itemCountStyle = m_guiSkin.GetStyle("ItemCount");
		itemCountStyle.fontSize = m_fontSize;

		for(int y = 0; y < 6; ++y)
		{
			for(int x = 0; x < (MasteryColumns-1); ++x)
			{
				if (GUI.Button(new Rect(padding+size*x+padding*x, size+padding+y*size+padding*y, size, size), icon) && m_champ.RemainMasteryPoint > 0)
				{
					m_champ.WeaponHolder.Evolution();
					m_champ.RemainMasteryPoint--;
				}
				GUI.Label(new Rect(padding+size*x+padding*x, size+padding+y*size+padding*y, size, size), "<color=white>0/0</color>", itemCountStyle);
			}
		}

		GUI.EndScrollView();
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
		displayMastery(new Rect(0, size, masteryWidth, Screen.height), 
		               Resources.Load<Texture2D>("Sprites/swordoftruth"), 
		               m_champ.m_creatureProperty.PhysicalAttackDamage, 
		               ref masterySrollPosition[0],
		               ()=>{
							m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=1;
							--m_champ.RemainStatPoint;
						}
		);

		displayMastery(new Rect(masteryWidth, size, masteryWidth, Screen.height), 
		               Resources.Load<Texture2D>("Sprites/staffoflight"), 
		               m_champ.m_creatureProperty.PhysicalDefencePoint, 
		               ref masterySrollPosition[1],
		               ()=>{
							m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=1;
							--m_champ.RemainStatPoint;
						}
		);

		displayMastery(new Rect(masteryWidth*2, size, masteryWidth, Screen.height), 
		               Resources.Load<Texture2D>("Sprites/robeofpower"), 
		               m_champ.m_creatureProperty.MaxHP, 
		               ref masterySrollPosition[2],
		               ()=>{
							m_champ.m_creatureProperty.AlphaMaxHP+=1;
							--m_champ.RemainStatPoint;
						}
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
