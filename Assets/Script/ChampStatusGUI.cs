using UnityEngine;
using System.Collections;

public class ChampStatusGUI : MonoBehaviour {

	Champ	m_champ;

	Rect 		m_guageWindowRect;
	Rect 		m_skillWindowRect;
	Rect		m_goodsWindowRect;

	Texture		m_guageTexture;

	[SerializeField]
	GUISkin		m_guiSkin = null;


	ChampSettingGUI	m_champSettingGUI = null;

	float[]		m_skillUsedTime = new float[4];

	void Start () {

		m_champ = transform.parent.gameObject.GetComponent<Champ>();
		m_guageTexture = Resources.Load<Texture>("Sprites/HP Guage");

		m_champSettingGUI = GameObject.Find("ChampSettingGUI").GetComponent<ChampSettingGUI>();

		m_goodsWindowRect = new Rect(Screen.width/2-Const.m_slotWidth*3/2, 0, Const.m_slotWidth*3, Const.m_slotHeight);
		m_guageWindowRect = new Rect(0, 0, Const.m_slotWidth*3, Const.m_slotHeight);

		float width = Const.m_slotWidth*0.7f;
		float height = Const.m_slotHeight*0.7f;
		m_skillWindowRect = new Rect(Screen.width/2-width, (Screen.height-height), (width+width), height);
	}

	void OnGUI()
	{		
		GUI.skin = m_guiSkin;
		m_guiSkin.label.fontSize = Const.m_fontSize;
		m_guiSkin.button.fontSize = Const.m_fontSize;

		//m_goodsWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");
		m_guageWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampGuage, m_guageWindowRect, DisplayStatusWindow, "");
		m_skillWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampSkill, m_skillWindowRect, DisplaySkillWindow, "");

		GUIStyle levelupStyle = m_guiSkin.GetStyle("LevelUp");
		levelupStyle.fontSize = Const.m_fontSize;

		GUIStyle itemCountStyle = m_guiSkin.GetStyle("ItemCount");
		itemCountStyle.fontSize = Const.m_fontSize;

		if (m_champ.RemainStatPoint > 0)
		{
		
			Rect levelUpButton = new Rect(Screen.width-Const.m_slotWidth*3, 0, Const.m_slotHeight, Const.m_slotHeight);

			if (GUI.Button(levelUpButton, "", levelupStyle))
			{
				ChampAbilityGUI abilityGUI = m_champ.transform.Find("ChampAbilityGUI").GetComponent<ChampAbilityGUI>();
				abilityGUI.gameObject.SetActive(true);
			}

			GUI.Label(levelUpButton, "<color=black>" + m_champ.RemainStatPoint +"</color>", itemCountStyle);
		}

		if (m_champ.ComboSkillStack > 0)
		{

			Rect levelUpButton = new Rect(Screen.width-Const.m_slotWidth, Const.m_slotHeight*1, Const.m_slotHeight, Const.m_slotHeight);

			Const.GuiButtonMultitouchable(levelUpButton, "", levelupStyle, ()=>{
				--m_champ.ComboSkillStack;
				m_champ.ApplyBuff(null, DamageDesc.BuffType.Combo100, 10f, null);
			});
			

			GUI.Label(new Rect(Screen.width-Const.m_slotWidth, Const.m_slotHeight*2, Const.m_slotHeight, Const.m_slotHeight), "<color=black>" + m_champ.ComboSkillStack +"</color>", itemCountStyle);
		}



		//DisplayGoodsWindow();
	}

	void drawGuage(Rect size, float ratio, string lable, Texture guage)
	{
		GUI.DrawTextureWithTexCoords(new Rect(size.x, size.y, size.width*ratio, size.height), guage, new Rect(0f, 0f, ratio, 1f));
		GUIStyle style = new GUIStyle();
		style.fontSize = Const.m_fontSize;
		style.normal.textColor = Color.grey;
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(size.x, size.y, size.width, size.height), lable, style);
		style.normal.textColor = Color.black;
		GUI.Label(new Rect(size.x+1, size.y+1, size.width, size.height), lable, style);
	}

	void DisplayGoodsWindow()
	{
		int size = (int)Const.m_slotWidth;
		int startX = size/3;
		int startY = 0;			
		
		ItemObject goldItemObj = Warehouse.Instance.Gold;

		GUI.BeginGroup(m_goodsWindowRect);
		GUI.Label(new Rect(startX, size*0.2f, size*0.7f, size*0.7f), goldItemObj.ItemIcon);
		GUI.Label(new Rect(startX+(size), 0, size, size), "<color=white>" + goldItemObj.Item.Count + "</color>");
		GUI.EndGroup();
	}

	//Setting up the Inventory window
	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)Const.m_slotHeight/3;

		float hp = m_champ.m_creatureProperty.getHPRemainRatio();
		string lable = Mathf.FloorToInt(m_champ.m_creatureProperty.HP).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxHP).ToString();
		drawGuage(new Rect(0, 0, m_guageWindowRect.width, size), hp, lable, m_guageTexture); 

		float expRatio = m_champ.m_creatureProperty.getExpRemainRatio();
		lable = Mathf.FloorToInt(m_champ.m_creatureProperty.Exp).ToString() + " / " + Mathf.FloorToInt(m_champ.m_creatureProperty.MaxExp).ToString();
		drawGuage(new Rect(0, startY+(size*1), m_guageWindowRect.width, size), expRatio, lable, m_guageTexture); 

		float chargingRatio = 1f;
		lable = "Weapon Enegy";
		drawGuage(new Rect(0, startY+(size*2), m_guageWindowRect.width, size), chargingRatio, lable, m_guageTexture); 

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
