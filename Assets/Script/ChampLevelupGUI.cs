using UnityEngine;
using System.Collections;

public class ChampLevelupGUI : MonoBehaviour {

	Creature	m_creature;
	Rect 		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
	Rect 		m_skillWindowRect = new Rect((Screen.width-100)/2, 0, 100, 30);

	[SerializeField]
	GUISkin		m_guiSkin;

	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();

	}

	void OnEnable() {
		Time.timeScale = 0;
	}

	void OnDisable() {
		Time.timeScale = 1;
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;

		m_statusWindowRect = GUI.Window (10, m_statusWindowRect, DisplayStatusWindow, "");
		m_skillWindowRect = GUI.Window (12, m_skillWindowRect, DisplaySkillWindow, "");		
	}

	IEnumerator UpdateDestroy()
	{
		yield return new WaitForSeconds(0.1f);

	}
	
	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = 60;

		if (GUI.Button(new Rect(Screen.width-size, 0, size, size), "X"))
		{
			this.gameObject.SetActive(false);
			return;
		}

		GUI.Label(new Rect(0, startY+(size*0), size, size), Resources.Load<Texture>("Sprites/level"));
		GUI.Label(new Rect(size, startY+(size*0), size, size), m_creature.m_creatureProperty.Level.ToString());


		GUI.Label(new Rect(0, startY+(size*1), size, size), Resources.Load<Texture>("Sprites/swordoftruth"));
		GUI.Label(new Rect(size, startY+(size*1), size, size), m_creature.m_creatureProperty.PAttackDamage.ToString());
		GUI.Button(new Rect(size+size, startY+(size*1), size, size), "+");

		GUI.Label(new Rect(0, startY+(size*2), size, size), Resources.Load<Texture>("Sprites/staffoflight"));
		GUI.Label(new Rect(size, startY+(size*2), size, size), m_creature.m_creatureProperty.PDefencePoint.ToString());
		GUI.Button(new Rect(size+size, startY+(size*2), size, size), "+");

		GUI.Label(new Rect(0, startY+(size*3), size, size), Resources.Load<Texture>("Sprites/robeofpower"));
		GUI.Label(new Rect(size, startY+(size*3), size, size), "0");
		GUI.Button(new Rect(size+size, startY+(size*3), size, size), "+");


		float expRatio = m_creature.m_creatureProperty.getExpRemainRatio();
		string lable = Mathf.FloorToInt(m_creature.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxExp).ToString();
		drawGuage(new Rect(size, startY+(size*4), 100-size, size), expRatio, lable, Resources.Load<Texture>("Sprites/HP Guage")); 

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
		int size = 25;

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
