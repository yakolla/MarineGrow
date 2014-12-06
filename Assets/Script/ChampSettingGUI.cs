using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ChampSettingGUI : MonoBehaviour {

	const int INVEN_SLOT_COLS = 4;
	const int INVEN_SLOT_ROWS = 4;
	const int EQUIP_ACCESSORY_SLOT_MAX = 4;

	[SerializeField]
	GameObject		m_prefChamp = null;

	[SerializeField]
	bool		m_cheat = true;

	[SerializeField]
	RefItemSpawn[]		m_itemSpawnDesc = null;

	ItemObject		m_equipedWeapon = null;
	ItemObject[]	m_aquipedAccessory = new ItemObject[EQUIP_ACCESSORY_SLOT_MAX];

	ItemObject		m_latestSelected = null;

	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;

	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);


	public ItemObject	EquipedWeapon
	{
		get {return m_equipedWeapon;}
	}

	public ItemObject[]	EquipedAccessories
	{
		get {return m_aquipedAccessory;}
	}

	void OnEnable() {
		Time.timeScale = 0;
		
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);

		if (m_cheat == true)
		{
			
		}
		else
		{
			//Load();
			
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData("Pref/Gun"));
			}
			
			//Save ();
		}
	}

	void OnDisable() {
		Time.timeScale = 1;
	}

	void OnGUI()
	{
		GUI.skin = m_guiSkin;

		m_statusWindowRect = GUI.Window (20, m_statusWindowRect, DisplayStatusWindow, "");	
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

	void DisplayItemDesc(ItemObject selectedItem, bool inEquipSlot)
	{
		int startY = 0;
		int size = (int)m_height;

		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*3), size, size), "Desc");
		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*4), size*3, size*3), selectedItem.Item.Description());

		if (selectedItem.Item.ItemType == ItemData.Type.Weapon)
		{
			if (true == inEquipSlot)
			{
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*6), size*2, size), "Unequip"))
				{
					m_equipedWeapon = null;				
				}
			}
			else
			{
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*6), size*2, size), "Equip"))
				{
					m_equipedWeapon = selectedItem;				
				}
			}
		}


		if (selectedItem.Item.LevelUpReqDescs.Count > 0)
		{
			bool canLevelup = true;
			ItemObject[] reqItems = new ItemObject[selectedItem.Item.LevelUpReqDescs.Count];
			
			for(int i = 0; i < reqItems.Length; ++i)
			{
				ItemData.LevelUpReqDesc desc = selectedItem.Item.LevelUpReqDescs[i];
				reqItems[i] = Warehouse.Instance.FindItemByItemType(desc.ItemType);
				if (reqItems[i] == null)
				{
					canLevelup = false;
				}
			}
			
			if (canLevelup == true)
			{
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+2)+size, startY+(size*6), size*2, size), "LevelUp"))
				{
					++selectedItem.Item.Level;
					foreach(ItemObject obj in reqItems)
					{
						Warehouse.Instance.RemoveItem(obj);
					}
				}
			}
		}


	}

	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;

		if (GUI.Button(new Rect(Screen.width-size, 0, size, size), "X") && m_equipedWeapon != null)
		{
			GameObject champObj = (GameObject)Instantiate(m_prefChamp, m_prefChamp.transform.position, m_prefChamp.transform.localRotation);
			m_equipedWeapon.Item.Use(champObj.GetComponent<Creature>());
			this.enabled = false;
			return;
		}

		GUI.Label(new Rect(0, startY+(size*0), size*2, size), "Weapon");
		if (GUI.Button(new Rect(0, startY+(size*1), size, size), m_equipedWeapon != null ? m_equipedWeapon.ItemIcon : null))
		{
			m_latestSelected = m_equipedWeapon;
		}

		GUI.Label(new Rect(size*2, startY+(size*0), size*2, size), "Accessory");

		for(int x = 0; x < EQUIP_ACCESSORY_SLOT_MAX; x++)
		{
			if (GUI.Button(new Rect(size*(2+x), startY+(size*1), size, size), ""))
			{
			}
		}

		GUI.Label(new Rect(0, startY+(size*2), size*2, size), "Items");

		for(int y = 0; y < INVEN_SLOT_ROWS; y++)
		{
			for(int x = 0; x < INVEN_SLOT_COLS; x++)
			{
				int id = x+y*INVEN_SLOT_COLS;
				ItemObject item = null;
				if (id < Warehouse.Instance.Items.Count)
					item = Warehouse.Instance.Items[id];				

				if (GUI.Button(new Rect(size*x, startY+(size*(3+y)), size, size), item != null ? item.ItemIcon : null))
				{
					if (item != null)
					{
						m_latestSelected = item;
					}
				}
			}
		}


		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*3), size, size), "Desc");
		if (m_latestSelected != null)
		{
			DisplayItemDesc(m_latestSelected, m_equipedWeapon == m_latestSelected);
		}

	}
}
