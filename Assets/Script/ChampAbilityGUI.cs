using UnityEngine;
using System.Collections;

public class ChampAbilityGUI : MonoBehaviour {

	Champ		m_champ;
	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;

	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);

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
		GUI.Label(new Rect(size, startY+(size*0), size, size), m_champ.RemainStatPoint.ToString());


		GUI.Label(new Rect(0, startY+(size*1), size, size), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(size, startY+(size*1), size, size), m_champ.m_creatureProperty.PhysicalAttackDamage.ToString());
		if (GUI.Button(new Rect(size+size, startY+(size*1), size, size), "+") && m_champ.RemainStatPoint > 0)
		{
			m_champ.m_creatureProperty.AlphaPhysicalAttackDamage+=1;
			--m_champ.RemainStatPoint;
		}

		GUI.Label(new Rect(0, startY+(size*2), size, size), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(size, startY+(size*2), size, size), m_champ.m_creatureProperty.PhysicalDefencePoint.ToString());
		if (GUI.Button(new Rect(size+size, startY+(size*2), size, size), "+") && m_champ.RemainStatPoint > 0)
		{
			m_champ.m_creatureProperty.AlphaPhysicalDefencePoint+=1;
			--m_champ.RemainStatPoint;
		}

		GUI.Label(new Rect(0, startY+(size*3), size, size), Resources.Load<Texture>("Sprites/robeofpower"));
		GUI.Label(new Rect(size, startY+(size*3), size, size), m_champ.m_creatureProperty.AlphaMaxHP.ToString());
		if (GUI.Button(new Rect(size+size, startY+(size*3), size, size), "+") && m_champ.RemainStatPoint > 0)
		{
			m_champ.m_creatureProperty.AlphaMaxHP+=1;
			--m_champ.RemainStatPoint;
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
		float hp = m_champ.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, 100, 15), hp, lable, Resources.Load<Texture>("Sprites/HP Guage")); 

	}


}