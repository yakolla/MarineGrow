using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;

	Rect 		m_guageWindowRect;
	Rect 		m_skillWindowRect;
	Rect		m_goodsWindowRect;

	Texture		m_guageTexture;
	float 		m_slotWidth = Screen.width * (1/5f);
	float 		m_slotHeight = Screen.height * (1/8f);

	[SerializeField]
	GUISkin		m_guiSkin = null;

	int			m_fontSize = (int)(Screen.width*(1/50f));

	ChampSettingGUI	m_champSettingGUI = null;

	float[]		m_skillUsedTime = new float[4];

	void Start () {

		m_champ = transform.parent.gameObject.GetComponent<Champ>();
		m_guageTexture = Resources.Load<Texture>("Sprites/HP Guage");

		m_champSettingGUI = GameObject.Find("ChampSettingGUI").GetComponent<ChampSettingGUI>();

		m_goodsWindowRect = new Rect(Screen.width/2-m_slotWidth, 0, m_slotWidth*2, m_slotHeight);
		m_guageWindowRect = new Rect(0, 0, m_slotWidth, m_slotHeight);

		float width = m_slotWidth*0.7f;
		float height = m_slotHeight*0.7f;
		m_skillWindowRect = new Rect(Screen.width/2-width, (Screen.height-height), (width+width), height);
	}

	void OnGUI()
	{		
		GUI.skin = m_guiSkin;
		m_guiSkin.label.fontSize = m_fontSize;
		m_guiSkin.button.fontSize = m_fontSize;

		//m_goodsWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");
		m_guageWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampGuage, m_guageWindowRect, DisplayStatusWindow, "");
		m_skillWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampSkill, m_skillWindowRect, DisplaySkillWindow, "");

		if (m_champ.RemainStatPoint > 0)
		{
			GUIStyle levelupStyle = m_guiSkin.GetStyle("LevelUp");
			levelupStyle.fontSize = m_fontSize;

			if (GUI.Button(new Rect(Screen.width-m_slotWidth, 0, m_slotHeight, m_slotHeight), "+", levelupStyle))
			{
				ChampAbilityGUI abilityGUI = m_champ.transform.Find("ChampAbilityGUI").GetComponent<ChampAbilityGUI>();
				abilityGUI.gameObject.SetActive(true);
			}
		}
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
	void DisplayGoodsWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_slotHeight/2;
		GUI.Label(new Rect(0, startY+(size*0), m_goodsWindowRect.width, size), "<color=white>Total Mob Kills:" + Warehouse.Instance.Stats.m_totalKills +  "</color>");
	}

	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_slotHeight/2;

		float hp = m_champ.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, m_slotWidth, size), hp, lable, m_guageTexture); 

		float expRatio = m_champ.m_creatureProperty.getExpRemainRatio();
		lable = Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();
		drawGuage(new Rect(0, startY+(size*1), m_slotWidth-size, size), expRatio, lable, m_guageTexture); 


	}

	void DisplaySkillWindow(int windowID)
	{
		int startX = 0;
		int size = (int)m_skillWindowRect.width/4;

		for(int i = 0; i < m_skillUsedTime.Length; ++i)
		{
			if (m_champSettingGUI.EquipedAccessories[i] == null)
				continue;

			Texture2D icon = m_champSettingGUI.EquipedAccessories[i].ItemIcon;
			if (icon == null)
				continue;

			if (GUI.Button(new Rect(startX+i*size, 0, size, size), icon ))
			{
				m_skillUsedTime[i] = Time.time;
				m_champSettingGUI.EquipedAccessories[i].Item.Use(m_champ);
			}

			float usedTime = m_skillUsedTime[i];
			if (usedTime == 0f)
				continue;
			float progress = Mathf.Min(1f, (Time.time - usedTime)/5f);
			GUI.Button(new Rect(startX+i*size, 0, size, size), Cooldown.ProgressUpdate(m_champSettingGUI.EquipedAccessories[i].ItemIcon, progress, new Color(1f, 1f, 1f, 0.5F)));
			if (progress == 1f)
				m_skillUsedTime[i] = 0f;
		}




	}
}
