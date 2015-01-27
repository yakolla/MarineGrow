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



	class CheckPriceDesc
	{
		public struct Desc
		{
			public bool 	able;
			public string 	colofulDesc;
		}

		public Desc	m_gemCond;
		public Desc	m_goldCond;
		public Desc	m_goldMedalCond;
		public Desc	m_weaponPartsCond;
		public Desc	m_weaponDNACond;
	}

	void GetPriceConditionDesc(RefPrice refPrice, CheckPriceDesc desc)
	{
		// gem
		desc.m_gemCond.able = refPrice.gem <= Warehouse.Instance.Gem;
		
		desc.m_gemCond.colofulDesc = desc.m_gemCond.able == true ? "<color=white>" : "<color=red>";
		desc.m_gemCond.colofulDesc += "Gems:" + refPrice.gem + "</color>";

		// gold
		desc.m_goldCond.able = refPrice.gold <= Warehouse.Instance.Gold;
		
		desc.m_goldCond.colofulDesc = desc.m_goldCond.able == true ? "<color=white>" : "<color=red>";
		desc.m_goldCond.colofulDesc += "Gold:" + refPrice.gold + "</color>";


		// gold medal
		desc.m_goldMedalCond.able = false;
		ItemObject goldMedalItemObj = Warehouse.Instance.FindItem(5);
		desc.m_goldMedalCond.able = goldMedalItemObj != null && refPrice.goldMedalItem <= goldMedalItemObj.Item.Count;
		
		desc.m_goldMedalCond.colofulDesc = desc.m_goldMedalCond.able == true ? "<color=white>" : "<color=red>";
		desc.m_goldMedalCond.colofulDesc += "Gold Medals:" + refPrice.goldMedalItem + "</color>";

		// levelup
		desc.m_weaponPartsCond.able = false;
		ItemObject levelupItemObj = Warehouse.Instance.FindItem(3);
		desc.m_weaponPartsCond.able = levelupItemObj != null && refPrice.weaponPartsItem <= levelupItemObj.Item.Count;
		
		desc.m_weaponPartsCond.colofulDesc = desc.m_weaponPartsCond.able == true ? "<color=white>" : "<color=red>";
		desc.m_weaponPartsCond.colofulDesc += "Weapon Parts:" + refPrice.weaponPartsItem + "</color>";

		// evolution
		desc.m_weaponDNACond.able = false;
		ItemObject evolutionItemObj = Warehouse.Instance.FindItem(4);
		desc.m_weaponDNACond.able = levelupItemObj != null && refPrice.weaponDNAItem <= evolutionItemObj.Item.Count;
		
		desc.m_weaponDNACond.colofulDesc = desc.m_weaponDNACond.able == true ? "<color=white>" : "<color=red>";
		desc.m_weaponDNACond.colofulDesc += "Weapon DNA:" + refPrice.weaponDNAItem + "</color>";
	}
	
	void DisplayItemDesc(ItemObject selectedItem, bool inEquipSlot)
	{
		int size = (int)m_slotHeight;
		int startX = size*(INVEN_SLOT_COLS+2);
		int startY = 0;


		GUI.Label(new Rect(startX, startY+(size*3), Screen.width-size*(INVEN_SLOT_COLS+1), size*3), selectedItem.Item.Description());

		if (selectedItem.Item.Lock == true)
		{

			if (selectedItem.Item.RefItem.price != null && selectedItem.Item.RefItem.price.unlock != null)
			{
				CheckPriceDesc checkPriceDesc = new CheckPriceDesc();

				GetPriceConditionDesc(selectedItem.Item.RefItem.price.unlock, checkPriceDesc);

				if (GUI.Button(new Rect(startX, startY+(size*6), size*2, size), "Unlock" + "\n" + checkPriceDesc.m_gemCond.colofulDesc))
				{
					if (checkPriceDesc.m_gemCond.able == true)
					{
						Warehouse.Instance.Gem -= selectedItem.Item.RefItem.price.unlock.gem;
						selectedItem.Item.Lock = false;
					}
				}
				else if (GUI.Button(new Rect(startX+size*2, startY+(size*6), size*2, size), "Unlock" + "\n" + checkPriceDesc.m_goldMedalCond.colofulDesc))
				{
					if (checkPriceDesc.m_goldMedalCond.able == true)
					{
						Warehouse.Instance.PullItem(5, selectedItem.Item.RefItem.price.unlock.goldMedalItem);
						selectedItem.Item.Lock = false;
					}
				}
			}
			return;
		}

		switch(selectedItem.Item.RefItem.type)
		{
		case ItemData.Type.Weapon:
		{
			if (true == inEquipSlot)
			{
				if (GUI.Button(new Rect(startX, startY+(size*6), size*2, size), "Unequip"))
				{
					m_equipedWeapon = null;				
				}
			}
			else
			{
				if (GUI.Button(new Rect(startX, startY+(size*6), size*2, size), "Equip"))
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

		if (selectedItem.Item.RefItem.levelUpItems.Count > 0)
		{
			if (selectedItem.Item.RefItem.price != null)
			{
				CheckPriceDesc checkPriceDesc = new CheckPriceDesc();
				
				GetPriceConditionDesc(selectedItem.Item.RefItem.price.levelup, checkPriceDesc);
				
				
				if (GUI.Button(new Rect(startX+size*2, startY+(size*6), size*2, size), "LevelUp" + "\n" + checkPriceDesc.m_weaponPartsCond.colofulDesc))
				{
					if (checkPriceDesc.m_weaponPartsCond.able == true)
					{
						++selectedItem.Item.Level;
						Warehouse.Instance.PullItem(3, selectedItem.Item.RefItem.price.levelup.weaponPartsItem);
					}
				}
			}

		}

		if (selectedItem.Item.RefItem.evolutionItems.Count > 0)
		{
			if (selectedItem.Item.RefItem.price != null)
			{

				CheckPriceDesc checkPriceDesc = new CheckPriceDesc();
				
				GetPriceConditionDesc(selectedItem.Item.RefItem.price.evolution, checkPriceDesc);

				if (GUI.Button(new Rect(startX+size*4, startY+(size*6), size*2, size), "Evolution" + "\n" + checkPriceDesc.m_weaponDNACond.colofulDesc))
				{
					if (checkPriceDesc.m_weaponDNACond.able == true)
					{
						++selectedItem.Item.Evolution;
						Warehouse.Instance.PullItem(4, selectedItem.Item.RefItem.price.evolution.weaponDNAItem);
					}
				}
			}
		}


	}

	[SerializeField]
	Vector2 itemScrollPosition;


	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_slotHeight;

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
			
			GUIStyle style = new GUIStyle();
			style.alignment = TextAnchor.LowerRight;
			style.richText = true;
			
			string str = "<color=white>" +item.Item.Count + "</color>";
			GUI.Label(new Rect(size*x, (size*(y)), size, size), str, style);

			++itemIndex;
		}
		GUI.EndScrollView();

		GUI.skin.label.alignment = TextAnchor.UpperLeft;

		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+2), startY+(size*2), size, size), "Desc");

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
