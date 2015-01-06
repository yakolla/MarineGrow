using UnityEngine;
using System.Collections;

public class ChampLevelupGUI : MonoBehaviour {

	Creature	m_creature;
	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;
	int			m_statPoint = 0;

	bool		m_stop = false;
	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);

	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
	}

	void OnEnable() {
		m_stop = true;
		m_statPoint+=3;
	}

	void OnDisable() {
		m_stop = false;
		Time.timeScale = 1;
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;

		m_statusWindowRect = GUI.Window (10, m_statusWindowRect, DisplayStatusWindow, "");	
	}

	void Update()
	{
		if (m_stop == true)
		{
			Time.timeScale = 0;
		}
	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;

		if (GUI.Button(new Rect(Screen.width-size, 0, size, size), "X"))
		{
			this.gameObject.SetActive(false);
			return;
		}

		GUI.Label(new Rect(0, startY+(size*0), size, size), Resources.Load<Texture>("Sprites/level"));
		GUI.Label(new Rect(size, startY+(size*0), size, size), m_statPoint.ToString());


		GUI.Label(new Rect(0, startY+(size*1), size, size), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(size, startY+(size*1), size, size), m_creature.m_creatureProperty.PhysicalAttackDamage.ToString());
		if (GUI.Button(new Rect(size+size, startY+(size*1), size, size), "+") && m_statPoint > 0)
		{
			m_creature.m_creatureProperty.AlphaPhysicalAttackDamage+=1;
			--m_statPoint;
		}

		GUI.Label(new Rect(0, startY+(size*2), size, size), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(size, startY+(size*2), size, size), m_creature.m_creatureProperty.PhysicalDefencePoint.ToString());
		if (GUI.Button(new Rect(size+size, startY+(size*2), size, size), "+") && m_statPoint > 0)
		{
			m_creature.m_creatureProperty.AlphaPhysicalDefencePoint+=1;
			--m_statPoint;
		}

		GUI.Label(new Rect(0, startY+(size*3), size, size), Resources.Load<Texture>("Sprites/robeofpower"));
		GUI.Label(new Rect(size, startY+(size*3), size, size), m_creature.m_creatureProperty.AlphaMaxHP.ToString());
		if (GUI.Button(new Rect(size+size, startY+(size*3), size, size), "+") && m_statPoint > 0)
		{
			m_creature.m_creatureProperty.AlphaMaxHP+=1;
			--m_statPoint;
		}

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
		float hp = m_creature.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_creature.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, 100, 15), hp, lable, Resources.Load<Texture>("Sprites/HP Guage")); 

	}


}
