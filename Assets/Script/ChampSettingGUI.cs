using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ChampSettingGUI : MonoBehaviour {

	const int INVEN_SLOT_COLS = 4;
	const int INVEN_SLOT_ROWS = 4;
	const int EQUIP_FOLLOWER_SLOT_MAX = 4;

	[SerializeField]
	GameObject		m_prefChamp = null;

	[SerializeField]
	bool		m_cheat = true;

	[SerializeField]
	RefItemSpawn[]		m_itemSpawnDesc = null;

	ItemObject		m_equipedWeapon = null;
	ItemObject[]	m_equipedFollowers = new ItemObject[EQUIP_FOLLOWER_SLOT_MAX];

	ItemObject		m_latestSelected = null;

	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;

	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_slotWidth = Screen.width * (1/5f);
	float 		m_slotHeight = Screen.height * (1/8f);


	public ItemObject	EquipedWeapon
	{
		get {return m_equipedWeapon;}
	}

	public ItemObject[]	EquipedAccessories
	{
		get {return m_equipedFollowers;}
	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
		
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);

		if (m_cheat == true)
		{
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(101));
				Warehouse.Instance.PushItem(new ItemWeaponData(102));
				Warehouse.Instance.PushItem(new ItemWeaponData(108));
				Warehouse.Instance.PushItem(new ItemWeaponData(111));
				Warehouse.Instance.PushItem(new ItemWeaponData(105));
				Warehouse.Instance.PushItem(new ItemWeaponData(106));
				Warehouse.Instance.PushItem(new ItemWeaponData(114));
				for(int i = 0 ; i < 1000; ++i)
				{
					Warehouse.Instance.PushItem(new ItemWeaponUpgradeFragmentData());
					Warehouse.Instance.PushItem(new ItemWeaponEvolutionFragmentData());
				}
				for(int i = 0 ; i < 200; ++i)
				{
					Warehouse.Instance.PushItem(new ItemGoldMedalData());
					Warehouse.Instance.PushItem(new ItemSilverMedalData());
				}

				Warehouse.Instance.Gold.Item.Count = 1000;
				Warehouse.Instance.Gem.Item.Count = 1000;

				Warehouse.Instance.PushItem(new ItemFollowerData(RefData.Instance.RefMobs[4]));
			}
		}
		else
		{
			//Load();
			
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(108));
			}
			
			//Save ();
		}
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;

		m_statusWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampInventory, m_statusWindowRect, DisplayStatusWindow, "");	
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd");

		Warehouse.Instance.Save(bf, file);

		file.Close();
	}

	public void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);

			Warehouse.Instance.Load(bf, file);

			file.Close();
		}
	}


	class CheckPriceDesc
	{
		public bool 	able;
		public string 	colofulDesc;	
	}

	bool GetPriceConditionDesc(RefPriceCondition refPriceCondition, CheckPriceDesc[] desc)
	{
		bool able = true;
		int i = 0;
		foreach(RefPrice refPrice in refPriceCondition.conds)
		{
			desc[i] = new CheckPriceDesc();
			RefItem refItem = RefData.Instance.RefItems[refPrice.refItemId];
			if (refItem == null)
			{
				Debug.Log("no refItemId:" + refPrice.refItemId);
				able = false;
				continue;
			}

			ItemObject itemObj = Warehouse.Instance.FindItem(refPrice.refItemId);
			if (itemObj != null && refPrice.count <= itemObj.Item.Count)
			{
				desc[i].able = true;
				desc[i].colofulDesc = "<color=white>";
			}
			else
			{
				desc[i].able = false;
				desc[i].colofulDesc = "<color=red>";
				able = false;
			}


			desc[i].colofulDesc += refItem.codeName + ":" + refPrice.count;
			desc[i].colofulDesc += "</color>";

			++i;
		}

		return able;
	}

	bool CheckAvailableItem(RefPriceCondition condition)
	{
		foreach(RefPrice price in condition.conds)
		{
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId);
			if (inventoryItemObj == null)
				return false;

			if (inventoryItemObj != null)
			{
				if (inventoryItemObj.Item.Count < price.count)
					return false;
			}
		}

		return true;
	}

	void PayPriceItem(RefPriceCondition condition)
	{
		foreach(RefPrice price in condition.conds)
		{
			Warehouse.Instance.PullItem(price.refItemId, price.count);
		}
	}

	delegate void OnPay();
	int makeItemButton(int startX, int startY, int size, RefPriceCondition[] conditions, string btnName, OnPay functor)
	{
		int prevWidth = 0;
		if (conditions != null && conditions.Length > 0)
		{
			foreach(RefPriceCondition condition in conditions)
			{
				int width = size*condition.conds.Length;
				GUI.BeginGroup(new Rect(startX+prevWidth, startY, width, size*2));
				if (GUI.Button(new Rect(0, 0, width, size*2), ""))
				{
					if (CheckAvailableItem(condition))
					{
						PayPriceItem(condition);
						functor();
					}
				}
				
				GUIStyle style = new GUIStyle();
				style.alignment = TextAnchor.MiddleCenter;
				style.richText = true;
				GUI.Label(new Rect(0, 0, width, size), "<color=white>"+btnName+"</color>", style);
				
				int priceIndex = 0;
				foreach(RefPrice price in condition.conds)
				{
					RefItem condRefItem = RefData.Instance.RefItems[price.refItemId];
					
					GUI.Label(new Rect(size*priceIndex, size, size, size), Resources.Load<Texture>(condRefItem.icon));
					style.alignment = TextAnchor.LowerRight;
					
					
					string str = "<color=white>";
					
					ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId);
					if (inventoryItemObj == null)
						str = "<color=red>";
					else if (inventoryItemObj != null && inventoryItemObj.Item.Count < price.count)
						str = "<color=red>";
					
					str += price.count + "</color>";
					GUI.Label(new Rect(size*priceIndex, size, size, size), str, style);
					
					++priceIndex;
					
				}
				GUI.EndGroup();
				
				prevWidth += width;
				
			}
			
		}

		return prevWidth;
	}
	
	void DisplayItemDesc(ItemObject selectedItem, bool inEquipSlot)
	{
		int size = (int)m_slotHeight;
		int startX = size*(INVEN_SLOT_COLS+4);
		int startY = size*2;


		GUI.Label(new Rect(startX, startY, Screen.width-size*(INVEN_SLOT_COLS+1), size*3), selectedItem.Item.Description());

		if (selectedItem.Item.Lock == true)
		{
			makeItemButton(startX, startY+(size*3), size, selectedItem.Item.RefItem.unlock, "Unlock", ()=>{
				selectedItem.Item.Lock = false;
			});

			return;
		}

		switch(selectedItem.Item.RefItem.type)
		{
		case ItemData.Type.Weapon:
		{
			if (true == inEquipSlot)
			{
				if (GUI.Button(new Rect(startX, startY+(size*2), size*2, size), "Unequip"))
				{
					m_equipedWeapon = null;				
				}
			}
			else
			{
				if (GUI.Button(new Rect(startX, startY+(size*2), size*2, size), "Equip"))
				{
					m_equipedWeapon = selectedItem;				
				}
			}
		}break;

		case ItemData.Type.Follower:
		{
			if (true == inEquipSlot)
			{
				if (GUI.Button(new Rect(startX, startY+(size*6), size*2, size), "Unfollower"))
				{
					for(int x = 0; x < m_equipedFollowers.Length; ++x)
					{
						if (m_equipedFollowers[x] != null)
						{
							m_equipedFollowers[x] = null;
							break;
						}
					}					
				}
			}
			else
			{
				if (GUI.Button(new Rect(startX, startY+(size*6), size*2, size), "Follower"))
				{
					bool aleadyExists = false;
					for(int x = 0; x < m_equipedFollowers.Length; ++x)
					{
						if (m_equipedFollowers[x] == selectedItem)
						{
							aleadyExists = true;
							break;
						}
					}	

					if (aleadyExists == false)
					{
						for(int x = 0; x < m_equipedFollowers.Length; ++x)
						{
							if (m_equipedFollowers[x] == null)
							{
								m_equipedFollowers[x] = selectedItem;
								break;
							}
						}	
					}

				}
			}
		}break;
		}
		int prevWidth = makeItemButton(startX, startY+(size*3), size, selectedItem.Item.RefItem.levelup, "Levelup", ()=>{
			++selectedItem.Item.Level;
		});
		makeItemButton(startX+prevWidth, startY+(size*3), size, selectedItem.Item.RefItem.evolution, "Evolution", ()=>{
			++selectedItem.Item.Evolution;
		});

	}

	[SerializeField]
	Vector2 itemScrollPosition;


	void DisplayStatusWindow(int windowID)
	{

		int size = (int)m_slotHeight;
		int startY = size;

		GUI.skin.label.alignment = TextAnchor.UpperLeft;

		if (GUI.Button(new Rect(Screen.width-size, 0, size, size), "X") && m_equipedWeapon != null)
		{
			GameObject champObj = (GameObject)Instantiate(m_prefChamp, m_prefChamp.transform.position, m_prefChamp.transform.localRotation);
			Creature champ = champObj.GetComponent<Creature>();

			m_equipedWeapon.Item.Use(champ);
			for(int x = 0; x < m_equipedFollowers.Length; ++x)
			{
				if (m_equipedFollowers[x] != null)
				{
					m_equipedFollowers[x].Item.Use(champ);
				}
			}	
			this.enabled = false;
			return;
		}

		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.LowerRight;
		style.richText = true;

		ItemObject goldItemObj = Warehouse.Instance.Gold;
		ItemObject gemItemObj = Warehouse.Instance.Gem;
		GUI.Label(new Rect(Screen.width/2-size*2, 0, size, size), goldItemObj.ItemIcon);
		GUI.Label(new Rect(Screen.width/2-size*2, 0, size, size), "<color=white>" +goldItemObj.Item.Count + "</color>", style);
		GUI.Label(new Rect(Screen.width/2-size, 0, size, size), gemItemObj.ItemIcon);
		GUI.Label(new Rect(Screen.width/2-size, 0, size, size), "<color=white>" +gemItemObj.Item.Count + "</color>", style);

		GUI.Label(new Rect(0, startY+(size*0), size*2, size), "Weapon");
		if (GUI.Button(new Rect(0, startY+(size*1), size, size), m_equipedWeapon != null ? m_equipedWeapon.ItemIcon : null))
		{
			m_latestSelected = m_equipedWeapon;
		}

		GUI.Label(new Rect(size*2, startY+(size*0), size*2, size), "Follower");
		for(int x = 0; x < EQUIP_FOLLOWER_SLOT_MAX; x++)
		{
			if (GUI.Button(new Rect(size*(2+x), startY+(size*1), size, size), m_equipedFollowers[x] != null ? m_equipedFollowers[x].ItemIcon : null))
			{
				m_latestSelected = m_equipedFollowers[x];
			}
		}

		GUI.Label(new Rect(0, startY+(size*2), size*2, size), "Items");
		itemScrollPosition = GUI.BeginScrollView(new Rect(0, startY+size+(size*2), (int)(size*4.5), size*4), itemScrollPosition, new Rect(0, 0, size*4, size+size*Warehouse.Instance.Items.Count/INVEN_SLOT_COLS));
		int itemIndex = 0;
		foreach(ItemObject item in Warehouse.Instance.Items)
		{
			int x = itemIndex%INVEN_SLOT_COLS;
			int y = itemIndex/INVEN_SLOT_COLS;
			if (GUI.Button(new Rect(size*x, (size*(y)), size, size), item.ItemIcon))
			{
				m_latestSelected = item;
			}
			
			string str = "<color=white>" +item.Item.Count + "</color>";
			GUI.Label(new Rect(size*x, (size*(y)), size, size), str, style);

			++itemIndex;
		}
		GUI.EndScrollView();

		GUI.skin.label.alignment = TextAnchor.UpperLeft;

		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+4), startY, size, size), "Desc");

		if (m_latestSelected != null)
		{
			bool equiped = m_equipedWeapon == m_latestSelected;
			if (equiped == false)
			{
				for(int x = 0; x < m_equipedFollowers.Length; ++x)
				{
					if (m_equipedFollowers[x] == m_latestSelected)
					{
						equiped = true;
						break;
					}
				}	
			}

			DisplayItemDesc(m_latestSelected, equiped);
		}

	}
}
