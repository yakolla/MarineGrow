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
				Warehouse.Instance.PushItem(new ItemWeaponData(106, null));
				ItemWeaponData itemWeaponData = new ItemWeaponData(108, null);
				itemWeaponData.Lock = false;
				Warehouse.Instance.PushItem(itemWeaponData);
				Warehouse.Instance.PushItem(new ItemWeaponData(111, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(114, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(118, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(120, null));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(1000));
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(1000));
				Warehouse.Instance.PushItem(new ItemGoldMedalData(200));
				Warehouse.Instance.PushItem(new ItemSilverMedalData(200));

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
				ItemWeaponData itemWeaponData = new ItemWeaponData(108, null);
				itemWeaponData.Lock = false;
				Warehouse.Instance.PushItem(itemWeaponData);
			}
			
			byte[] data = Warehouse.Instance.Serialize();
			Warehouse.Instance.Deserialize(data);
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

		m_statusWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampInventory, m_statusWindowRect, DisplayStatusWindow, "");
		m_goodsWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			Application.LoadLevel("Worldmap");
		}
	}

	float getItemLevelupWorth(ItemObject itemObj)
	{
		return itemObj.Item.Level + (itemObj.Item.Level-1) * 7f;
	}

	float getItemEvolutionWorth(ItemObject itemObj)
	{
		return itemObj.Item.Evolution+1;
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
			Const.makeItemButton(m_guiSkin, m_fontSize, startX, startY, size, selectedItem.Item.RefItem.unlock, 1f, "Unlock", ()=>{
				selectedItem.Item.Lock = false;
				switch(selectedItem.Item.RefItem.id)
				{
				case 101:
					GPlusPlatform.Instance.ReportProgress(
						"CgkIrKGfsOUeEAIQAg", 100, (bool success) => {
					});  
					break;
				case 102:
					GPlusPlatform.Instance.ReportProgress(
						"CgkIrKGfsOUeEAIQBw", 100, (bool success) => {
					});  
					break;
				case 111:
					GPlusPlatform.Instance.ReportProgress(
						"CgkIrKGfsOUeEAIQCA", 100, (bool success) => {
					});  
					break;
				case 106:
					GPlusPlatform.Instance.ReportProgress(
						"CgkIrKGfsOUeEAIQCQ", 100, (bool success) => {
					});  
					break;
				case 118:
					GPlusPlatform.Instance.ReportProgress(
						"CgkIrKGfsOUeEAIQCg", 100, (bool success) => {
					});  
					break;
				case 120:
					GPlusPlatform.Instance.ReportProgress(
						"CgkIrKGfsOUeEAIQCw", 100, (bool success) => {
					});  
					break;
				}

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
							if (m_equipedAccessories[x].Item.Compare(selectedItem.Item))
							{
								m_equipedAccessories[x] = null;
								break;
							}

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

		if (selectedItem.Item.Level < Const.ItemMaxLevel)
		{
			Const.makeItemButton(m_guiSkin, m_fontSize, startX+size*3, startY, size, selectedItem.Item.RefItem.levelup, getItemLevelupWorth(selectedItem), "Levelup", ()=>{
				++selectedItem.Item.Level;
			});
		}
		/*else
		{
			Const.makeItemButton(m_guiSkin, m_fontSize, startX+size*3, startY, size, selectedItem.Item.RefItem.evolution, getItemEvolutionWorth(selectedItem), "Evolution", ()=>{
				++selectedItem.Item.Evolution;
				selectedItem.Item.Level = 1;
			});
		}*/


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

			m_equipedWeapon.Item.Equip(champ);
			for(int x = 0; x < m_equipedAccessories.Length; ++x)
			{
				if (m_equipedAccessories[x] != null)
				{
					m_equipedAccessories[x].Item.Equip(champ);
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


		for(int itemIndex = 0; itemIndex < Warehouse.Instance.Items.Count; ++itemIndex)
		{
			ItemObject item = Warehouse.Instance.Items[itemIndex];

			int x = itemIndex%INVEN_SLOT_COLS;
			int y = itemIndex/INVEN_SLOT_COLS;
			if (GUI.Button(new Rect(size*x, (size*2*(y)+y*size), size*2, size*2), item.ItemIcon))
			{
				m_latestSelected = item;
			}

			string str = "<color=white>" +item.Item.Count + "</color>";
			GUI.Label(new Rect(size*x, (size*2*(y)+y*size), size*2, size*2), str, itemCountStyle);

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
