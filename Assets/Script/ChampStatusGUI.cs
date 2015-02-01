using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Creature	m_creature;

	Rect 		m_guageWindowRect;
	Rect 		m_skillWindowRect;
	Rect		m_goodsWindowRect;

	Texture		m_guageTexture;
	float 		m_slotWidth = Screen.width * (1/5f);
	float 		m_slotHeight = Screen.height * (1/8f);

	[SerializeField]
	GUISkin		m_guiSkin = null;

	int			m_fontSize = (int)(Screen.width*(1/50f));

	void Start () {

		m_creature = transform.parent.gameObject.GetComponent<Creature>();
		m_guageTexture = Resources.Load<Texture>("Sprites/HP Guage");

		m_goodsWindowRect = new Rect(Screen.width/2-m_slotWidth, 0, m_slotWidth*2, m_slotHeight);
		m_guageWindowRect = new Rect(Screen.width/2-m_slotWidth/2, Screen.height-m_slotHeight, m_slotWidth, m_slotHeight);
		m_skillWindowRect = new Rect(Screen.width/2-m_slotWidth, Screen.height-m_slotHeight-m_slotHeight, m_slotWidth+m_slotWidth, m_slotHeight);
	}

	void OnGUI()
	{		
		GUI.skin = m_guiSkin;
		m_guiSkin.label.fontSize = m_fontSize;
		m_guiSkin.button.fontSize = m_fontSize;

		m_goodsWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");
		m_guageWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampGuage, m_guageWindowRect, DisplayStatusWindow, "");
		m_skillWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampSkill, m_skillWindowRect, DisplaySkillWindow, "");
	}

	void drawGuage(Rect size, float ratio, string lable, Texture guage)
	{
		GUI.DrawTextureWithTexCoords(new Rect(size.x, size.y, size.width*ratio, size.height), guage, new Rect(0f, 0f, ratio, 1f));
		GUIStyle style = new GUIStyle();
		style.fontSize = m_fontSize;
		style.normal.textColor = Color.grey;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(size.x, size.y, size.width, size.height), lable, style);
		style.normal.textColor = Color.black;
		GUI.Label(new Rect(size.x+1, size.y+1, size.width, size.height), lable, style);
	}

	static public void DisplayChampGoodsGUI(int size)
	{
		int startX = size/3;
		int startY = 0;	

		
		ItemObject goldItemObj = Warehouse.Instance.Gold;
		ItemObject gemItemObj = Warehouse.Instance.Gem;
		GUI.Label(new Rect(startX, size*0.2f, size*0.7f, size*0.7f), goldItemObj.ItemIcon);
		GUI.Label(new Rect(startX+(size), 0, size, size), "<color=white>" + goldItemObj.Item.Count + "</color>");
		GUI.Label(new Rect(startX+(size)*3, size*0.2f, size*0.7f, size*0.7f), gemItemObj.ItemIcon);
		GUI.Label(new Rect(startX+(size)*4, 0, size, size), "<color=white>" +gemItemObj.Item.Count + "</color>");
	}

	void DisplayGoodsWindow(int windowID)
	{
		DisplayChampGoodsGUI((int)m_slotHeight);
	}

	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_slotHeight/2;

		float hp = m_creature.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_creature.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, m_slotWidth, size), hp, lable, m_guageTexture); 

		float expRatio = m_creature.m_creatureProperty.getExpRemainRatio();
		lable = Mathf.FloorToInt(m_creature.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_creature.m_creatureProperty.MaxExp).ToString();
		drawGuage(new Rect(0, startY+(size*1), m_slotWidth-size, size), expRatio, lable, m_guageTexture); 

	}

	void DisplaySkillWindow(int windowID)
	{
		int startX = 0;
		int size = (int)m_skillWindowRect.width/4;
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
