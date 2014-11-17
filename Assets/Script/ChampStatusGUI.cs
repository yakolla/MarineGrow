using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Creature	m_creature;

	Rect 		m_guageWindowRect;
	Rect 		m_skillWindowRect;
	Texture		m_guageTexture;
	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/12f);

	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();
		m_guageTexture = Resources.Load<Texture>("Sprites/HP Guage");


		m_guageWindowRect = new Rect((Screen.width-m_width)/2, Screen.height-m_height, m_width, m_height);
		m_skillWindowRect = new Rect((Screen.width-m_width)/2, Screen.height-m_height-m_height, m_width, m_height);
	}

	void OnGUI()
	{		
		m_guageWindowRect = GUI.Window (1, m_guageWindowRect, DisplayGuageWindow, "");		
		m_skillWindowRect = GUI.Window (2, m_skillWindowRect, DisplaySkillWindow, "");		
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
		int startY = 0;
		int size = (int)m_height/2;

		float hp = m_creature.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_creature.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, m_width, size), hp, lable, m_guageTexture); 

		float expRatio = m_creature.m_creatureProperty.getExpRemainRatio();
		lable = Mathf.FloorToInt(m_creature.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxExp).ToString();
		drawGuage(new Rect(0, startY+(size*1), m_width-size, size), expRatio, lable, m_guageTexture); 

	}
	IEnumerator spawnObject(GameObject pref, Vector3 pos)
	{
		GameObject spwan = (GameObject)Instantiate(pref, pos, transform.rotation);
		spwan.SetActive(false);
		yield return new WaitForSeconds (0.3f);
		spwan.SetActive(true);
	}
	void DisplaySkillWindow(int windowID)
	{
		int startX = 0;
		int size = (int)m_width/4;

		if (GUI.Button(new Rect(startX+0*size, 0, size, size), "Z"))
		{
			GameObject prefFollower = Resources.Load<GameObject>("Pref/Follower");
			Vector3 pos = transform.position;
			StartCoroutine(spawnObject(prefFollower, pos));

		}
		GUI.Button(new Rect(startX+1*size, 0, size, size), "X");
		GUI.Button(new Rect(startX+2*size, 0, size, size), "C");
		GUI.Button(new Rect(startX+3*size, 0, size, size), "V");
	}
}
