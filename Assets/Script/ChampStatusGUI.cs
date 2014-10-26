using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Creature	m_creature;

	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();

	}

	Rect statusWindowRect = new Rect(0, Screen.height-100, 100, 100);
	Rect guageWindowRect = new Rect(300, Screen.height-30, 100, 30);
	void OnGUI()
	{		
		statusWindowRect = GUI.Window (0, statusWindowRect, DisplayStatusWindow, "Status");
		guageWindowRect = GUI.Window (1, guageWindowRect, DisplayGuageWindow, "Guage");		
	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		GUI.Label(new Rect(0, 15, 30, 30), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(30, 15, 30, 30), m_creature.m_creatureProperty.getDamage().ToString());
		GUI.Label(new Rect(0, 45, 30, 30), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(30, 45, 30, 30), "0");
		GUI.Label(new Rect(0, 75, 30, 30), Resources.Load<Texture>("Sprites/robeofpower"));
		GUI.Label(new Rect(30, 75, 30, 30), "0");
	}

	//Setting up the Inventory window
	void DisplayGuageWindow(int windowID)
	{
		Creature creature = transform.parent.gameObject.GetComponent<Creature>();
		float hp = creature.m_creatureProperty.getHPRemainRatio();
		Debug.Log(hp);
		GUI.DrawTextureWithTexCoords(new Rect(0, 0, 100*hp, 15), Resources.Load<Texture>("Sprites/HP Guage"), new Rect(0f, 0f, hp, 1f));
	}
}
