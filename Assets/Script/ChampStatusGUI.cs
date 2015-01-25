using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Creature	m_creature;

	Rect 		m_guageWindowRect;
	Rect 		m_skillWindowRect;
	Rect		m_goodsWindowRect;

	Texture		m_guageTexture;
	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/12f);

	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();
		m_guageTexture = Resources.Load<Texture>("Sprites/HP Guage");

		m_goodsWindowRect = new Rect((Screen.width-m_width)/2, 0, m_width, m_height);
		m_guageWindowRect = new Rect((Screen.width-m_width)/2, Screen.height-m_height, m_width, m_height);
		m_skillWindowRect = new Rect((Screen.width-m_width)/2, Screen.height-m_height-m_height, m_width, m_height);
	}

	void OnGUI()
	{		
		m_goodsWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");
		m_guageWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampGuage, m_guageWindowRect, DisplayStatusWindow, "");
		m_skillWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampSkill, m_skillWindowRect, DisplaySkillWindow, "");
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

	void DisplayGoodsWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;
		
		//GUI.Label(new Rect(0, 0, m_goodsWindowRect.width, m_goodsWindowRect.height), "G:" + Warehouse.Instance.Gold);

		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.LowerRight;
		style.richText = true;
		
		ItemObject goldItemObj = Warehouse.Instance.Gold;
		ItemObject gemItemObj = Warehouse.Instance.Gem;
		GUI.Label(new Rect(0, 0, size, size), goldItemObj.ItemIcon);
		GUI.Label(new Rect(0, 0, size, size), "<color=white>" +goldItemObj.Item.Count + "</color>", style);
		GUI.Label(new Rect(size*2, 0, size, size), gemItemObj.ItemIcon);
		GUI.Label(new Rect(size*2, 0, size, size), "<color=white>" +gemItemObj.Item.Count + "</color>", style);
	}

	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
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

	void DisplaySkillWindow(int windowID)
	{
		int startX = 0;
		int size = (int)m_width/4;
		if (GUI.Button(new Rect(startX+0*size, 0, size, size), "Z"))
		{
			GameObject prefFollower = Resources.Load<GameObject>("Pref/Follower");
			Instantiate(prefFollower, transform.position, transform.rotation);
		}
		if (GUI.Button(new Rect(startX+1*size, 0, size, size), "X"))
		{
			m_creature.m_creatureProperty.Heal((int)m_creature.m_creatureProperty.MaxHP);
		}
		GUI.Button(new Rect(startX+2*size, 0, size, size), "C");
		GUI.Button(new Rect(startX+3*size, 0, size, size), "V");
	}
}
