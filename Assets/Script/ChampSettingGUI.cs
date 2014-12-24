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

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);


	public ItemObject	EquipedWeapon
	{
		get {return m_equipedWeapon;}
	}

	public ItemObject[]	EquipedAccessories
	{
		get {return m_equipedFollowers;}
	}

	void OnEnable() {
		Time.timeScale = 0;
		
		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);

		if (m_cheat == true)
		{
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(101));
				Warehouse.Instance.PushItem(new ItemWeaponData(102));
				Warehouse.Instance.PushItem(new ItemWeaponData(108));
				Warehouse.Instance.PushItem(new ItemWeaponData(104));
				Warehouse.Instance.PushItem(new ItemWeaponData(105));
				Warehouse.Instance.PushItem(new ItemWeaponData(106));
				Warehouse.Instance.PushItem(new ItemWeaponUpgradeFragmentData(11));
				Warehouse.Instance.PushItem(new ItemWeaponEvolutionFragmentData(11));
				Warehouse.Instance.PushItem(new ItemFollowerData(1001));
				Warehouse.Instance.PushItem(new ItemFollowerData(1002));
				Warehouse.Instance.PushItem(new ItemFollowerData(1003));
				Warehouse.Instance.PushItem(new ItemFollowerData(1004));
			}
		}
		else
		{
			//Load();
			
			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(103));
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

	bool canProgressUpItem(List<RefProgressUpItem> progressUpItems)
	{
		bool canLevelup = true;

		foreach(RefProgressUpItem levelUpItem in progressUpItems)
		{
			ItemObject haveItemObj = Warehouse.Instance.FindItem(levelUpItem.refItemId);
			if (haveItemObj == null)
			{
				canLevelup = false;
				continue;
			}
			
			if (haveItemObj != null && haveItemObj.Item.Count < levelUpItem.count )
			{
				canLevelup = false;
				continue;
			}
		}

		return canLevelup;
	}
	
	void DisplayItemDesc(ItemObject selectedItem, bool inEquipSlot)
	{
		int startY = 0;
		int size = (int)m_height;

		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*3), size, size), "Desc");
		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*4), size*3, size*3), selectedItem.Item.Description());

		switch(selectedItem.Item.RefItem.type)
		{
		case ItemData.Type.Weapon:
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
		}break;

		case ItemData.Type.Follower:
		{
			if (true == inEquipSlot)
			{
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*6), size*2, size), "Unfollower"))
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
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*6), size*2, size), "Follower"))
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

		if (selectedItem.Item.RefItem.levelUpItems.Count > 0)
		{
			bool canLevelup = canProgressUpItem(selectedItem.Item.RefItem.levelUpItems);
			
			if (canLevelup == true)
			{
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+2)+size, startY+(size*6), size*2, size), "LevelUp"))
				{
					++selectedItem.Item.Level;
					foreach(RefProgressUpItem levelUpItem in selectedItem.Item.RefItem.levelUpItems)
					{
						Warehouse.Instance.PullItem(levelUpItem.refItemId, levelUpItem.count);
					}
				}
			}

			bool canEvolution = canProgressUpItem(selectedItem.Item.RefItem.evolutionItems);
			if (canEvolution == true)
			{
				if (GUI.Button(new Rect(size*(INVEN_SLOT_COLS+4)+size, startY+(size*6), size*2, size), "Evolution"))
				{
					++selectedItem.Item.Evolution;
					foreach(RefProgressUpItem levelUpItem in selectedItem.Item.RefItem.evolutionItems)
					{
						Warehouse.Instance.PullItem(levelUpItem.refItemId, levelUpItem.count);
					}
				}
			}
		}


	}

	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;

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

		for(int y = 0; y < INVEN_SLOT_ROWS; y++)
		{
			for(int x = 0; x < INVEN_SLOT_COLS; x++)
			{
				int id = x+y*INVEN_SLOT_COLS;
				ItemObject item = null;
				if (id < Warehouse.Instance.Items.Count)
				{
					item = Warehouse.Instance.Items[id];				

					if (GUI.Button(new Rect(size*x, startY+(size*(3+y)), size, size), item != null ? item.ItemIcon : null))
					{
						m_latestSelected = item;
					}

					GUIStyle style = new GUIStyle();
					style.alignment = TextAnchor.LowerRight;
					style.richText = true;

					string str = "<color=white>" +item.Item.Count + "</color>";
					GUI.Label(new Rect(size*x, startY+(size*(3+y)), size, size), str, style);

				}
			}
		}

		GUI.skin.label.alignment = TextAnchor.UpperLeft;

		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*3), size, size), "Desc");
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
