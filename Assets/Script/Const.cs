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

	public const int ItemMaxLevel = 9;
	public const int ShowMaxDamageNumber = 3;

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
	public static int makeItemButton(GUISkin guiSkin, int fontSize, int startX, int startY, int size, RefPriceCondition condition, float itemWorth, string btnName, OnPay functor)
	{
		int prevWidth = 0;
		if (condition != null)
		{			
			prevWidth = size*3;
			if (false == makeItemButtonCore(guiSkin, fontSize, startX, startY, prevWidth, size, size*2, condition.conds, itemWorth, btnName, functor))
			{
				if (condition.else_conds != null)
					makeItemButtonCore(guiSkin, fontSize, startX, startY+size*2, prevWidth, size, size, condition.else_conds, itemWorth, "", functor);
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
			
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId, null);
			int hasCount = 0;
			if (inventoryItemObj == null)
			{
				str = "<color=red>";
				able = false;
			}
			else if (inventoryItemObj != null)
			{
				if (inventoryItemObj.Item.Count < price.count*itemWorth)
				{
					str = "<color=red>";
					able = false;
				}
				hasCount = inventoryItemObj.Item.Count;
			}
			str += hasCount;
			str += "/" + price.count*itemWorth;
			str += "</color>";
			GUI.Label(new Rect(width/(conds.Length)*priceIndex, startY+height, width/(conds.Length), imgSize), str, itemCountStyle);
			
			++priceIndex;
			
		}
		
		GUI.EndGroup();
		
		return able;
		
	}
}
