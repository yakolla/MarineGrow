using UnityEngine;

public class Const {

	public enum GUI_WindowID
	{
		ChampGuage,
		ChampSkill,
		ChampLevelUp,
		ChampInventory,
		ChampGoods,
		PopupShop,
		MainMenu,
	}


	public static float 		m_slotWidth = Screen.width * (1/14f);
	public static float 		m_slotHeight = Screen.height * (1/8f);
	

	public static int		m_fontSize = (int)(Screen.width*(1/50f));

	public const int ItemMaxLevel = 9;
	public const int ShowMaxDamageNumber = 3;
	public const int ComboSkillStackOnCombo = 100;
	public const int ComboKill_2 = 200;
	public const int ComboKill_3 = 300;


	public const string ACH_UNLOCKED_THE_FIREGUN = "CgkIrKGfsOUeEAIQAg";
	public const string ACH_KILL_THE_BOSS_OF_STAGE_1 = "CgkIrKGfsOUeEAIQAw";
	public const string ACH_SURVIVAL_DURATION_10_WAVES = "CgkIrKGfsOUeEAIQBA";
	public const string ACH_SURVIVAL_DURATION_20_WAVES = "CgkIrKGfsOUeEAIQBQ";
	public const string ACH_SURVIVAL_DURATION_30_WAVES = "CgkIrKGfsOUeEAIQBg";
	public const string ACH_UNLOCKED_THE_LIGHTNINGLAUNCHER = "CgkIrKGfsOUeEAIQBw";
	public const string ACH_UNLOCKED_THE_ROCKETLAUNCHER = "CgkIrKGfsOUeEAIQCA";
	public const string ACH_UNLOCKED_THE_GUIDEDROCKETLAUNCHER = "CgkIrKGfsOUeEAIQCQ";
	public const string ACH_UNLOCKED_THE_LASERBEAM = "CgkIrKGfsOUeEAIQCg";
	public const string ACH_UNLOCKED_THE_BOOMERANGLAUNCHER = "CgkIrKGfsOUeEAIQCw";
	public const string ACH_KILL_THE_BOSS_OF_STAGE_2 = "CgkIrKGfsOUeEAIQDA";
	public const string ACH_KILL_THE_BOSS_OF_STAGE_3 = "CgkIrKGfsOUeEAIQDQ";
	public const string ACH_KILL_THE_BOSS_OF_STAGE_4 = "CgkIrKGfsOUeEAIQDg";
	public const string ACH_KILL_THE_BOSS_OF_STAGE_5 = "CgkIrKGfsOUeEAIQDw";
	public const string ACH_DISCOVER_THE_BOSS_OF_STAGE_1 = "CgkIrKGfsOUeEAIQEA";
	public const string ACH_DISCOVER_THE_BOSS_OF_STAGE_2 = "CgkIrKGfsOUeEAIQEQ";
	public const string ACH_DISCOVER_THE_BOSS_OF_STAGE_3 = "CgkIrKGfsOUeEAIQEg";
	public const string ACH_DISCOVER_THE_BOSS_OF_STAGE_4 = "CgkIrKGfsOUeEAIQEw";
	public const string ACH_DISCOVER_THE_BOSS_OF_STAGE_5 = "CgkIrKGfsOUeEAIQFA";
	public const string ACH_COMBO_KILLS_100 = "CgkIrKGfsOUeEAIQFQ";
	public const string ACH_COMBO_KILLS_200 = "CgkIrKGfsOUeEAIQFg";
	public const string ACH_COMBO_KILLS_300 = "CgkIrKGfsOUeEAIQFw";
	public const string LEAD_COMBO_MAX_KILLS = "CgkIrKGfsOUeEAIQAQ";



	public static bool CheckAvailableItem(RefPrice[] conds, float itemWorth)
	{
		foreach(RefPrice price in conds)
		{
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId, null);
			if (inventoryItemObj == null)
				return false;
			
			if (inventoryItemObj != null)
			{
				if (inventoryItemObj.Item.Count < price.count*itemWorth)
					return false;
			}
		}
		
		return true;
	}
	
	public static void PayPriceItem(RefPrice[] conds, float itemWorth)
	{
		foreach(RefPrice price in conds)
		{
			Warehouse.Instance.PullItem(Warehouse.Instance.FindItem(price.refItemId, null), (int)(price.count*itemWorth));
		}
	}
	
	public delegate void OnPay();
	public static int makeItemButton(GUISkin guiSkin, int fontSize, int startX, int startY, int width, int height, RefPriceCondition condition, float itemWorth, string btnName, OnPay functor)
	{
		int prevWidth = 0;
		if (condition != null)
		{			
			prevWidth = width*3;
			int btnHeight = height;
			if (btnName.Equals("") == false)
			{
				btnHeight += height;
			}

			if (false == makeItemButtonCore(guiSkin, fontSize, startX, startY, prevWidth, height, btnHeight, condition.conds, itemWorth, btnName, functor))
			{
				if (condition.else_conds != null)
					makeItemButtonCore(guiSkin, fontSize, startX, startY+width*2, prevWidth, height, height, condition.else_conds, itemWorth, "", functor);
			}
			
		}
		
		return prevWidth;
	}
	public static bool makeItemButtonCore(GUISkin guiSkin, int fontSize, int startX, int startY, int width, int height, int btnHeight, RefPrice[] conds, float itemWorth, string btnName, OnPay functor)
	{
		GUIStyle itemCountStyle = guiSkin.GetStyle("ItemReqCount");
		itemCountStyle.fontSize = fontSize;
		
		GUI.BeginGroup(new Rect(startX, startY, width, btnHeight));
		
		if (GUI.Button(new Rect(0, 0, width, btnHeight), ""))
		{
			if (CheckAvailableItem(conds, itemWorth))
			{
				PayPriceItem(conds, itemWorth);
				functor();
			}
		}
		
		
		GUI.Label(new Rect(0, 0, width, height), "<color=white>"+btnName+"</color>");
		
		bool able = true;
		int priceIndex = 0;
		startY = height/4;
		float imgSize = height*0.7f;
		height = btnHeight-height;
		foreach(RefPrice price in conds)
		{
			RefItem condRefItem = RefData.Instance.RefItems[price.refItemId];
			
			GUI.Label(new Rect((width/(1+conds.Length)-imgSize/2)+(width/(1+conds.Length)-imgSize/2)*2*priceIndex, height, imgSize, imgSize), Resources.Load<Texture>(condRefItem.icon));
			
			string str = "<color=white>";
			int cost = (int)(price.count*itemWorth);
			
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId, null);
			int hasCount = 0;
			if (inventoryItemObj == null)
			{
				str = "<color=red>";
				able = false;
			}
			else if (inventoryItemObj != null)
			{
				if (inventoryItemObj.Item.Count < cost)
				{
					str = "<color=red>";
					able = false;
				}
				hasCount = inventoryItemObj.Item.Count;
			}
			str += hasCount;
			str += "/" + cost;
			str += "</color>";
			GUI.Label(new Rect(width/(conds.Length)*priceIndex, startY+height, width/(conds.Length), imgSize), str, itemCountStyle);
			
			++priceIndex;
			
		}
		
		GUI.EndGroup();
		
		return able;
		
	}

	public static void GuiButtonMultitouchable(Rect rect, string name, GUIStyle style, System.Action callback)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
			{
				if (GUI.Button(rect, name, style))
				{
					callback();
				}
			}
			break;
		default:
			{
				foreach(Touch t in Input.touches)
				{
					if (t.phase == TouchPhase.Ended)
					{
						Vector2 vec = t.position;
						vec.y = Screen.height - vec.y; // You need to invert since GUI and screen have differnet coordinate system
						if(rect.Contains(vec))// Do something
						{
							callback();
						}
					}
				}
				
				GUI.Label(rect, name, style);
			}
			break;
		}

	}
}
