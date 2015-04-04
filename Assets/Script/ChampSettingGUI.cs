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

	float 		m_slotWidth = Screen.width * (1/14f);
	float 		m_slotHeight = Screen.height * (1/8f);

	int		m_fontSize = (int)(Screen.width*(1/50f));

	int			m_stage = 1;

	Spawn		m_spawn;

	float		touchedDelta = 0f;
	string		log;

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
		m_spawn = GameObject.Find("Dungeon/Spawn").GetComponent<Spawn>();
		GPlusPlatform.Instance.InitAnalytics(GameObject.Find("GAv3").GetComponent<GoogleAnalyticsV3>());

		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
		m_goodsWindowRect = new Rect(Screen.width/2-m_slotWidth*3, 0, m_slotWidth*6, m_slotHeight);
		
		if (m_cheat == true)
		{
			if (Warehouse.Instance.Items.Count == 0)
			{
				ItemWeaponData gunWeaponData = new ItemWeaponData(108, null);
				gunWeaponData.Lock = false;				
				Warehouse.Instance.PushItem(gunWeaponData);

				Warehouse.Instance.PushItem(new ItemWeaponData(101, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(102, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(106, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(111, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(120, null));

				Warehouse.Instance.PushItem(new ItemWeaponData(114, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(118, null));

				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(11, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(12, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(13, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(14, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(15, 500));
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(500));
				Warehouse.Instance.PushItem(new ItemGoldMedalData(200));
				Warehouse.Instance.PushItem(new ItemSilverMedalData(200));

				ItemAccessoryData bootsData = new ItemAccessoryData(10);
				bootsData.OptionDescs.Add(new ItemMagicOption(ItemData.Option.Weapon, 125));
				Warehouse.Instance.PushItem(bootsData);

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
				ItemWeaponData gunWeaponData = new ItemWeaponData(108, null);
				gunWeaponData.Lock = false;				
				Warehouse.Instance.PushItem(gunWeaponData);
				
				Warehouse.Instance.PushItem(new ItemWeaponData(101, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(102, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(106, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(111, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(120, null));

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

		//m_goodsWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampGoods, m_goodsWindowRect, DisplayGoodsWindow, "");
		m_statusWindowRect = GUI.Window ((int)Const.GUI_WindowID.ChampInventory, m_statusWindowRect, DisplayStatusWindow, "");


		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			Application.LoadLevel("Worldmap");
		}
	}

	float getItemLevelupWorth(ItemObject itemObj, RefPriceCondition refPriceCondition)
	{
		float pricePerLevel = 1f;
		if (refPriceCondition != null)
			pricePerLevel = refPriceCondition.pricePerLevel;

		return 1f + (itemObj.Item.Level-1) * pricePerLevel;
	}

	float getItemEvolutionWorth(ItemObject itemObj)
	{
		return itemObj.Item.Evolution+1;
	}


	
	void DisplayItemDesc(ItemObject selectedItem, bool inEquipSlot, int startX, int startY, int width, int height)
	{

		GUIStyle	itemDescStyle = m_guiSkin.GetStyle("Desc");
		itemDescStyle.fontSize = m_fontSize;

		int size = (int)m_slotWidth;

		GUI.BeginGroup(new Rect(startX, startY, width, height));
		GUI.Label(new Rect(0, 0, width, height), selectedItem.Item.Description(),itemDescStyle);
		GUI.EndGroup();

		startX += width;

		if (selectedItem.Item.Lock == true)
		{
			Const.makeItemButton(m_guiSkin, m_fontSize, startX, startY, size, size, selectedItem.Item.RefItem.unlock, 1f, "Unlock", ()=>{
				selectedItem.Item.Lock = false;
				switch(selectedItem.Item.RefItem.id)
				{
				case 101:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_FIREGUN, 100, (bool success) => {
					});  
					break;
				case 102:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_LIGHTNINGLAUNCHER, 100, (bool success) => {
					});  
					break;
				case 111:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_ROCKETLAUNCHER, 100, (bool success) => {
					});  
					break;
				case 106:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_GUIDEDROCKETLAUNCHER, 100, (bool success) => {
					});  
					break;
				case 118:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_LASERBEAM, 100, (bool success) => {
					});  
					break;
				case 120:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_BOOMERANGLAUNCHER, 100, (bool success) => {
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
			Const.makeItemButton(m_guiSkin, m_fontSize, startX+size*3, startY, size, size, selectedItem.Item.RefItem.levelup, getItemLevelupWorth(selectedItem, selectedItem.Item.RefItem.levelup), "Levelup", ()=>{
				++selectedItem.Item.Level;

				GPlusPlatform.Instance.AnalyticsTrackEvent("Weapon", "Levelup", selectedItem.Item.RefItem.codeName + "_Lv:" + selectedItem.Item.Level, 0);
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

	void DisplayGoodsWindow()
	{
		int size = (int)m_slotWidth;
		int startX = size/3;
		int startY = 0;	
		
		
		ItemObject goldItemObj = Warehouse.Instance.Gold;
		ItemObject gemItemObj = Warehouse.Instance.Gem;

		GUI.BeginGroup(m_goodsWindowRect);
		GUI.Label(new Rect(startX, size*0.2f, size*0.7f, size*0.7f), goldItemObj.ItemIcon);
		GUI.Label(new Rect(startX+(size), 0, size, size), "<color=white>" + goldItemObj.Item.Count + "</color>");
		GUI.Label(new Rect(startX+(size)*3, size*0.2f, size*0.7f, size*0.7f), gemItemObj.ItemIcon);
		GUI.Label(new Rect(startX+(size)*4, 0, size, size), "<color=white>" +gemItemObj.Item.Count + "</color>");
		GUI.EndGroup();
	}

	void DisplayStatusWindow(int windowID)
	{
		DisplayGoodsWindow();

		int size = (int)m_slotWidth;
		int startY = 0;
		int maxStage = m_spawn.GetStage(Warehouse.Instance.WaveIndex);
		int stageStartX = (size*7)/2;
		if (GUI.Button(new Rect(stageStartX, Screen.height-size*2, size, size), "+"))
		{
			++m_stage;
			m_stage = Mathf.Min(maxStage, m_stage);
		}
		if (GUI.Button(new Rect(stageStartX, Screen.height-size, size, size), "-"))
		{
			--m_stage;
			m_stage = Mathf.Max(1, m_stage);
		}

		GUI.Label(new Rect(stageStartX+size, Screen.height-size-size/2, size*2, size), "Stage:" + m_stage + "/" + maxStage);

		Const.makeItemButton(m_guiSkin, m_fontSize, stageStartX+size*4, Screen.height-size*2, size, (int)m_slotHeight, RefData.Instance.RefItems[1003].levelup, (m_stage-1)*(m_stage-1)*(m_stage-1), "Start", ()=>{
			if (m_equipedWeapon != null)
			{
				GameObject champObj = (GameObject)Instantiate(m_prefChamp, m_prefChamp.transform.position, m_prefChamp.transform.localRotation);
				GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/champ_skin");
				
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

				m_spawn.StartWave(m_stage-1);

				GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", "Stage:"+m_stage, 0);
				GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", m_equipedWeapon.Item.RefItem.codeName+"_Lv:"+m_equipedWeapon.Item.Level, 0);


				this.enabled = false;

			}
		});

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

		if(Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];

			if (touch.position.normalized.y >= 0.2f)
			{
				if (touch.phase == TouchPhase.Moved)
				{
					touchedDelta = touch.deltaPosition.y;
					accel = Input.acceleration.y*5f;
				}

				log = "" + touch.position.normalized.y;
			}
		}
		itemScrollPosition.y += touchedDelta * accel;
		accel -= accel*Time.deltaTime;
		accel = Mathf.Max(0f, accel);

		GUI.Label(new Rect(0, startY+(size*2), size*2, size), "<color=white>Items</color>", columnStyle);
		int paddingY = size/2;

		switch (Application.platform)
		{
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
			itemScrollPosition = GUI.BeginScrollView(new Rect(0, startY+size+(size*2)-paddingY, Screen.width, m_slotHeight*3.5f), itemScrollPosition, new Rect(0, 0, Screen.width-size, size+size*2*Warehouse.Instance.Items.Count/INVEN_SLOT_COLS+Warehouse.Instance.Items.Count/INVEN_SLOT_COLS*size));
			break;
		default:
			itemScrollPosition = GUI.BeginScrollView(new Rect(0, startY+size+(size*2)-paddingY, Screen.width, m_slotHeight*3.5f), itemScrollPosition, new Rect(0, 0, Screen.width-size, size+size*2*Warehouse.Instance.Items.Count/INVEN_SLOT_COLS+Warehouse.Instance.Items.Count/INVEN_SLOT_COLS*size),GUIStyle.none,GUIStyle.none);
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
