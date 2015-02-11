using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ChampSettingGUI : MonoBehaviour {

	const int INVEN_SLOT_COLS = 1;
	const int INVEN_SLOT_ROWS = 4;
	const int EQUIP_ACCESSORY_SLOT_MAX = 4;

	[SerializeField]
	GameObject		m_prefChamp = null;

	[SerializeField]
	bool		m_cheat = true;

	[SerializeField]
	RefItemSpawn[]		m_itemSpawnDesc = null;

	ItemObject		m_equipedWeapon = null;
	ItemObject[]	m_equipedAccessories = new ItemObject[EQUIP_ACCESSORY_SLOT_MAX];

	ItemObject		m_latestSelected = null;

	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;
	Rect		m_goodsWindowRect;

	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_slotWidth = Screen.width * (1/5f);
	float 		m_slotHeight = Screen.height * (1/8f);

	int		m_fontSize = (int)(Screen.width*(1/50f));

	public ItemObject	EquipedWeapon
	{
		get {return m_equipedWeapon;}
	}

	public ItemObject[]	EquipedAccessories
	{
		get {return m_equipedAccessories;}
	}

	void Start()
	{
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
		m_goodsWindowRect = new Rect(Screen.width/2-m_slotWidth, 0, m_slotWidth*2, m_slotHeight);
		
		if (m_cheat == true)
		{
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(101, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(102, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(108, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(111, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(114, null));
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

				Warehouse.Instance.PushItem(new ItemAccessoryData(10));

				Warehouse.Instance.Gold.Item.Count = 100000;
				Warehouse.Instance.Gem.Item.Count = 100000;

				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					Warehouse.Instance.PushItem(new ItemFollowerData(follower));
				}

			}
		}
		else
		{
			//Load();
			
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(108, null));
			}
			
			//Save ();
		}


	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;
		m_guiSkin.label.fontSize = m_fontSize;
		m_guiSkin.button.fontSize = m_fontSize;

		m_statusWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampInventory, m_statusWindowRect, DisplayStatusWindow, "");
		m_goodsWindowRect = GUI.Window ((int)GUIConst.WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");
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

	float getItemLevelupWorth(ItemObject itemObj)
	{
		return itemObj.Item.Level * 5;
	}

	float getItemEvolutionWorth(ItemObject itemObj)
	{
		return (itemObj.Item.Evolution+1) * 50;
	}

	bool CheckAvailableItem(RefPrice[] conds, float itemWorth)
	{
		foreach(RefPrice price in conds)
		{
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId);
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

	void PayPriceItem(RefPrice[] conds, float itemWorth)
	{
		foreach(RefPrice price in conds)
		{
			Warehouse.Instance.PullItem(price.refItemId, (int)(price.count*itemWorth));
		}
	}

	delegate void OnPay();
	int makeItemButton(int startX, int startY, int size, RefPriceCondition condition, float itemWorth, string btnName, OnPay functor)
	{
		int prevWidth = 0;
		if (condition != null)
		{			
			prevWidth = size*3;
			if (false == makeItemButtonCore(startX, startY, prevWidth, size, size*2, condition.conds, itemWorth, btnName, functor))
			{
				makeItemButtonCore(startX, startY+size*2, prevWidth, size, size, condition.else_conds, itemWorth, "", functor);
			}
			
		}
		
		return prevWidth;
	}
	bool makeItemButtonCore(int startX, int startY, int width, int height, int btnHeight, RefPrice[] conds, float itemWorth, string btnName, OnPay functor)
	{
		GUIStyle itemCountStyle = m_guiSkin.GetStyle("ItemReqCount");
		itemCountStyle.fontSize = m_fontSize;

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
			
			ItemObject inventoryItemObj = Warehouse.Instance.FindItem(price.refItemId);
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
	
	void DisplayItemDesc(ItemObject selectedItem, bool inEquipSlot, int startX, int startY, int width, int height)
	{

		GUIStyle	itemDescStyle = m_guiSkin.GetStyle("Desc");
		itemDescStyle.fontSize = m_fontSize;

		int size = (int)m_slotHeight;

		GUI.BeginGroup(new Rect(startX, startY, width, height));
		GUI.Label(new Rect(0, 0, width, height), selectedItem.Item.Description(),itemDescStyle);
		GUI.EndGroup();

		startX += width;

		if (selectedItem.Item.Lock == true)
		{
			makeItemButton(startX, startY, size, selectedItem.Item.RefItem.unlock, 1f, "Unlock", ()=>{
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
				if (GUI.Button(new Rect(startX, startY, size*3, size*2), ""))
				{
					m_equipedWeapon = null;				
				}
				GUI.Label(new Rect(startX, startY, size*3, size*2), "<color=white>"+"Unequip"+"</color>");
			}
			else
			{
				if (GUI.Button(new Rect(startX, startY, size*3, size*2), ""))
				{
					m_equipedWeapon = selectedItem;				
				}
				GUI.Label(new Rect(startX, startY, size*3, size*2), "<color=white>"+"Equip"+"</color>");
			}
		}break;
		
		case ItemData.Type.Accessory:
		case ItemData.Type.Follower:
		{
			if (true == inEquipSlot)
			{
				if (GUI.Button(new Rect(startX, startY, size*3, size*2), "Unfollower"))
				{
					for(int x = 0; x < m_equipedAccessories.Length; ++x)
					{
						if (m_equipedAccessories[x] != null)
						{
							m_equipedAccessories[x] = null;
							break;
						}
					}					
				}
			}
			else
			{
				if (GUI.Button(new Rect(startX, startY, size*3, size*2), "Follower"))
				{
					bool aleadyExists = false;
					for(int x = 0; x < m_equipedAccessories.Length; ++x)
					{
						if (m_equipedAccessories[x] == selectedItem)
						{
							aleadyExists = true;
							break;
						}
					}	

					if (aleadyExists == false)
					{
						for(int x = 0; x < m_equipedAccessories.Length; ++x)
						{
							if (m_equipedAccessories[x] == null)
							{
								m_equipedAccessories[x] = selectedItem;
								break;
							}
						}	
					}

				}
			}
		}break;
		}

		if (selectedItem.Item.Level < 9)
		{
			makeItemButton(startX+size*3, startY, size, selectedItem.Item.RefItem.levelup, getItemLevelupWorth(selectedItem), "Levelup", ()=>{
				++selectedItem.Item.Level;
			});
		}
		else
		{
			makeItemButton(startX+size*3, startY, size, selectedItem.Item.RefItem.evolution, getItemEvolutionWorth(selectedItem), "Evolution", ()=>{
				++selectedItem.Item.Evolution;
				selectedItem.Item.Level = 1;
			});
		}


	}

	[SerializeField]
	Vector2 itemScrollPosition;
	
	float accel = 1f;

	void DisplayGoodsWindow(int windowID)
	{
		ChampStatusGUI.DisplayChampGoodsGUI((int)m_slotHeight);
	}

	void DisplayStatusWindow(int windowID)
	{

		int size = (int)m_slotHeight;
		int startY = 0;

		if (GUI.Button(new Rect(Screen.width/2-size, Screen.height-size, size*2, size), "Start") && m_equipedWeapon != null)
		{
			GameObject champObj = (GameObject)Instantiate(m_prefChamp, m_prefChamp.transform.position, m_prefChamp.transform.localRotation);
			GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/marine_skin");

			GameObject enemyBody = Instantiate (prefEnemyBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
			enemyBody.name = "Body";
			enemyBody.transform.parent = champObj.transform;
			enemyBody.transform.localPosition = Vector3.zero;
			enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;

			Creature champ = champObj.GetComponent<Creature>();

			m_equipedWeapon.Item.Use(champ);
			for(int x = 0; x < m_equipedAccessories.Length; ++x)
			{
				if (m_equipedAccessories[x] != null)
				{
					m_equipedAccessories[x].Item.Use(champ);
				}
			}	
			this.enabled = false;
			return;
		}

		GUIStyle columnStyle = m_guiSkin.GetStyle("Column");
		columnStyle.fontSize = m_fontSize;


		GUI.Label(new Rect(0, startY+(size*0), size*2, size), "<color=white>Weapon</color>", columnStyle);
		if (GUI.Button(new Rect(0, startY+(size*1), size, size), m_equipedWeapon != null ? m_equipedWeapon.ItemIcon : null))
		{
			m_latestSelected = m_equipedWeapon;
		}

		GUI.Label(new Rect(size*2, startY+(size*0), size*2, size), "<color=white>Accessory</color>", columnStyle);
		for(int x = 0; x < EQUIP_ACCESSORY_SLOT_MAX; x++)
		{
			if (GUI.Button(new Rect(size*(2+x), startY+(size*1), size, size), m_equipedAccessories[x] != null ? m_equipedAccessories[x].ItemIcon : null))
			{
				m_latestSelected = m_equipedAccessories[x];
			}
		}



		GUIStyle itemCountStyle = m_guiSkin.GetStyle("ItemCount");
		itemCountStyle.fontSize = m_fontSize;

		float delta = 0f;
		if(Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Moved)
			{
				delta = touch.deltaPosition.y;
				accel = -Input.acceleration.y*5f;
			}
		}
		itemScrollPosition.y += delta * accel;
		accel -= accel*Time.deltaTime;
		GUI.Label(new Rect(0, startY+(size*2), size*2, size), "<color=white>Items</color>", columnStyle);

		switch (Application.platform)
		{
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
			itemScrollPosition = GUI.BeginScrollView(new Rect(0, startY+size+(size*2), Screen.width, size*4), itemScrollPosition, new Rect(0, 0, Screen.width-size, size+size*2*Warehouse.Instance.Items.Count/INVEN_SLOT_COLS+Warehouse.Instance.Items.Count/INVEN_SLOT_COLS*size));
			break;
		default:
			itemScrollPosition = GUI.BeginScrollView(new Rect(0, startY+size+(size*2), Screen.width, size*4), itemScrollPosition, new Rect(0, 0, Screen.width-size, size+size*2*Warehouse.Instance.Items.Count/INVEN_SLOT_COLS+Warehouse.Instance.Items.Count/INVEN_SLOT_COLS*size),GUIStyle.none,GUIStyle.none);
			break;
		}

		int itemIndex = 0;
		foreach(ItemObject item in Warehouse.Instance.Items)
		{
			int x = itemIndex%INVEN_SLOT_COLS;
			int y = itemIndex/INVEN_SLOT_COLS;
			if (GUI.Button(new Rect(size*x, (size*2*(y)+y*size), size*2, size*2), item.ItemIcon))
			{
				m_latestSelected = item;
			}

			string str = "<color=white>" +item.Item.Count + "</color>";
			GUI.Label(new Rect(size*x, (size*2*(y)+y*size), size*2, size*2), str, itemCountStyle);

			++itemIndex;

			bool equiped = m_equipedWeapon == item;
			if (equiped == false)
			{
				for(int e = 0; e < m_equipedAccessories.Length; ++e)
				{
					if (m_equipedAccessories[e] == item)
					{
						equiped = true;
						break;
					}
				}	
			}
			
			DisplayItemDesc(item, equiped, size*3, size*2*(y)+y*size, size*4, size*2);
		}
		GUI.EndScrollView();


	}
}
