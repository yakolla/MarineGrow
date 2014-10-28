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
		m_statusWindowRect = GUI.Window (0, m_statusWindowRect, DisplayStatusWindow, "Status");
		m_guageWindowRect = GUI.Window (1, m_guageWindowRect, DisplayGuageWindow, "Guage");		
	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 15;
		int size = 30;

		GUI.Label(new Rect(0, startY, size, size), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(size, startY, size, size), m_creature.m_creatureProperty.getDamage().ToString());
		GUI.Button(new Rect(m_statusWindowRect.width-size, startY, size, size), "+");

		GUI.Label(new Rect(0, startY+size, size, size), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(size, startY+size, size, size), "0");
		GUI.Button(new Rect(m_statusWindowRect.width-size, startY+size, size, size), "+");

		GUI.Label(new Rect(0, startY+(size*2), size, size), Resources.Load<Texture>("Sprites/robeofpower"));
		GUI.Label(new Rect(size, startY+(size*2), size, size), "0");
		GUI.Button(new Rect(m_statusWindowRect.width-size, startY+(size*2), size, size), "+");
	}

	//Setting up the Inventory window
	void DisplayGuageWindow(int windowID)
	{

		float hp = m_creature.m_creatureProperty.getHPRemainRatio();

		GUI.DrawTextureWithTexCoords(new Rect(0, 0, 100*hp, 15), Resources.Load<Texture>("Sprites/HP Guage"), new Rect(0f, 0f, hp, 1f));
		string gague = Mathf.FloorToInt(m_creature.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxHP).ToString();
		GUIStyle style = new GUIStyle();
		style.fontSize = 10;
		style.normal.textColor = Color.grey;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0, 0, 100, 15), gague, style);
		style.normal.textColor = Color.black;
		GUI.Label(new Rect(1, 1, 100, 15), gague, style);
	}
}
