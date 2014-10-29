using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Creature	m_creature;
	Rect 		m_statusWindowRect = new Rect(0, Screen.height-100, 100, 100);
	Rect 		m_guageWindowRect = new Rect(300, Screen.height-30, 100, 30);


	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();

	}

	void OnGUI()
	{		
		m_statusWindowRect = GUI.Window (0, m_statusWindowRect, DisplayStatusWindow, "");
		m_guageWindowRect = GUI.Window (1, m_guageWindowRect, DisplayGuageWindow, "");		
	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = 20;

		GUI.Label(new Rect(0, startY+(size*0), size, size), Resources.Load<Texture>("Sprites/level"));
		GUI.Label(new Rect(size, startY+(size*0), size, size), m_creature.m_creatureProperty.Level.ToString());

		GUI.Label(new Rect(0, startY+(size*1), size, size), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(size, startY+(size*1), size, size), m_creature.m_creatureProperty.PAttackDamage.ToString());

		GUI.Label(new Rect(0, startY+(size*2), size, size), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(size, startY+(size*2), size, size), m_creature.m_creatureProperty.PDefencePoint.ToString());

		GUI.Label(new Rect(0, startY+(size*3), size, size), Resources.Load<Texture>("Sprites/robeofpower"));
		GUI.Label(new Rect(size, startY+(size*3), size, size), "0");

		float expRatio = m_creature.m_creatureProperty.getExpRemainRatio();
		string lable = Mathf.FloorToInt(m_creature.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxExp).ToString();
		drawGuage(new Rect(size, startY+(size*4), size, size), expRatio, lable, Resources.Load<Texture>("Sprites/HP Guage")); 

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
