using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class ChampSettingGUI : MonoBehaviour {

	const int EQUIP_ACCESSORY_SLOT_MAX = 4;

	GameObject	m_inventoryObj;
	YGUISystem.GUIImageStatic	m_gold;
	YGUISystem.GUIImageStatic	m_gem;
	YGUISystem.GUIButton	m_weapon;
	YGUISystem.GUIButton[]	m_accessories = new YGUISystem.GUIButton[EQUIP_ACCESSORY_SLOT_MAX];

	const int INVEN_SLOT_COLS = 1;
	const int INVEN_SLOT_ROWS = 4;


	[SerializeField]
	GameObject		m_prefChamp = null;

	[SerializeField]
	Transform		m_spawnChamp;

	[SerializeField]
	bool		m_cheat = true;

	[SerializeField]
	RefItemSpawn[]		m_itemSpawnDesc = null;

	ItemObject		m_equipedWeapon = null;
	ItemObject[]	m_equipedAccessories = new ItemObject[EQUIP_ACCESSORY_SLOT_MAX];

	ItemObject		m_latestSelected = null;

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
		
		if (m_cheat == true)
		{
			if (Warehouse.Instance.Items.Count == 0)
			{
				WeaponStat gunStat = new WeaponStat();
				gunStat.firingCount = 2; 
				ItemWeaponData gunWeaponData = new ItemWeaponData(108, gunStat);

				gunWeaponData.Lock = false;				
				Warehouse.Instance.PushItem(gunWeaponData);

				for(int w = 101; w < 128; ++w)
				{
					if (w == 108)
						continue;

					Warehouse.Instance.PushItem(new ItemWeaponData(w, null));
				}


				Warehouse.Instance.PushItem(new ItemWeaponPartsData(3, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(11, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(12, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(13, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(14, 500));
				Warehouse.Instance.PushItem(new ItemWeaponPartsData(15, 500));
				Warehouse.Instance.PushItem(new ItemWeaponDNAData(500));
				Warehouse.Instance.PushItem(new ItemGoldMedalData(200));
				Warehouse.Instance.PushItem(new ItemXPPotionData(200));

				ItemAccessoryData bootsData = new ItemAccessoryData(10);
				bootsData.OptionDescs.Add(new ItemMagicOption(ItemData.Option.Weapon, 125));
				Warehouse.Instance.PushItem(bootsData);

				Warehouse.Instance.Gold.Item.Count = 100000;
				Warehouse.Instance.Gem.Item.Count = 10000;

				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					ItemFollowerData followerData = new ItemFollowerData(follower.id);
					followerData.Lock = true;
					Warehouse.Instance.PushItem(followerData);
				}

			}
		}
		else
		{
			//Load();
			
			if (Warehouse.Instance.Items.Count == 0)
			{
				WeaponStat gunStat = new WeaponStat();
				gunStat.firingCount = 2; 
				ItemWeaponData gunWeaponData = new ItemWeaponData(108, gunStat);
				gunWeaponData.Lock = false;	
				Warehouse.Instance.PushItem(gunWeaponData);
				
				Warehouse.Instance.PushItem(new ItemWeaponData(101, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(102, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(106, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(111, null));
				Warehouse.Instance.PushItem(new ItemWeaponData(120, null));

				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					ItemFollowerData followerData = new ItemFollowerData(follower.id);
					followerData.Lock = true;
					Warehouse.Instance.PushItem(followerData);
				}

			}
			
			byte[] data = Warehouse.Instance.Serialize();
			Warehouse.Instance.Deserialize(data);
		}

		m_weapon = new YGUISystem.GUIButton(transform.Find("WeaponButton").gameObject, ()=>{return true;});
		for(int i = 0; i < m_accessories.Length; ++i)
		{
			m_accessories[i] = new YGUISystem.GUIButton(transform.Find("AccessoryButton" + i).gameObject, ()=>{return true;});
		}

		m_gold = new YGUISystem.GUIImageStatic(transform.Find("GoldImage").gameObject, Warehouse.Instance.Gold.ItemIcon);
		m_gem = new YGUISystem.GUIImageStatic(transform.Find("GemImage").gameObject, Warehouse.Instance.Gem.ItemIcon);

		m_inventoryObj = transform.Find("InvScrollView/Contents").gameObject;
		RectTransform rectInventoryObj = m_inventoryObj.GetComponent<RectTransform>();
		Vector2 rectContents = new Vector2(	rectInventoryObj.rect.width, 0);

		GameObject prefGUIInventorySlot = Resources.Load<GameObject>("Pref/GUIInventorySlot");
		RectTransform	rectGUIInventorySlot = prefGUIInventorySlot.GetComponent<RectTransform>();

		for(int itemIndex = 0; itemIndex < Warehouse.Instance.Items.Count; ++itemIndex)
		{
			ItemObject item = Warehouse.Instance.Items[itemIndex];

			GameObject obj = Instantiate(prefGUIInventorySlot) as GameObject;
			GUIInventorySlot invSlot = obj.GetComponent<GUIInventorySlot>();

			obj.transform.parent = m_inventoryObj.transform;
			obj.transform.localScale = prefGUIInventorySlot.transform.localScale;
			obj.transform.localPosition = new Vector3(0f, rectGUIInventorySlot.rect.height-rectGUIInventorySlot.rect.height*itemIndex, 0);

			invSlot.Init(item.ItemIcon, item.Item.Description());
			invSlot.PriceButton0.m_enable = false;
			invSlot.PriceButton1.m_enable = false;

			int capturedItemIndex = itemIndex;

			if (item.Item.Lock == true)
			{
				if (item.Item.RefItem.unlock != null)
				{
					SetButtonRole(ButtonRole.Unlock, invSlot, invSlot.PriceButton0, itemIndex);
				}
			}
			else
			{
				switch(item.Item.RefItem.type)
				{
				case ItemData.Type.Weapon:
				case ItemData.Type.Accessory:
				case ItemData.Type.Follower:
					SetButtonRole(ButtonRole.Equip, invSlot, invSlot.PriceButton0, itemIndex);
					break;
				}

			}

			if (item.Item.RefItem.levelup != null)
			{
				SetButtonRole(ButtonRole.Levelup, invSlot, invSlot.PriceButton1, itemIndex);
			}



		}
		rectContents.y = rectGUIInventorySlot.rect.height*2*Warehouse.Instance.Items.Count;
		rectInventoryObj.sizeDelta = rectContents;
		rectInventoryObj.position = new Vector3(rectInventoryObj.position.x, 0, rectInventoryObj.position.z);
	}
	enum ButtonRole
	{
		Nothing,
		Equip,
		Unequip,
		Levelup,
		Unlock,
	}
	void SetButtonRole(ButtonRole role, GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, int itemIndex)
	{
		ItemObject item = Warehouse.Instance.Items[itemIndex];
		priceGemButton.RemoveAllListeners();

		switch(role)
		{
		case ButtonRole.Equip:
			{
				priceGemButton.m_enable = true;

				priceGemButton.SetPrices(null, null);
			priceGemButton.AddListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );
				priceGemButton.SetLable("Equip");
			}
			break;
		case ButtonRole.Unequip:
			{
				priceGemButton.m_enable = true;

				priceGemButton.SetPrices(null, null);
			priceGemButton.AddListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );
				priceGemButton.SetLable("Unequip");
			}
			break;

		case ButtonRole.Levelup:
			{

				priceGemButton.m_enable = item.Item.RefItem.levelup.conds != null;
				
				priceGemButton.SetPrices(item.Item.RefItem.levelup.conds, item.Item.RefItem.levelup.else_conds);

			priceGemButton.AddListener(() => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );
				priceGemButton.SetLable("Levelup");
			}
			break;

		case ButtonRole.Unlock:
			{
				priceGemButton.m_enable = item.Item.RefItem.unlock.conds != null;

				priceGemButton.SetPrices(item.Item.RefItem.unlock.conds, item.Item.RefItem.unlock.else_conds);
			priceGemButton.AddListener(() => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickUnlock(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );
				priceGemButton.SetLable("Unlock");
			}
			break;

		default:
			{
			}
			break;
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
		m_gold.Lable.Text.text = Warehouse.Instance.Gold.Item.Count.ToString();
		m_gem.Lable.Text.text = Warehouse.Instance.Gem.Item.Count.ToString();

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			Application.LoadLevel("Worldmap");
		}
	}

	void PopupShop()
	{
		GameObject.Find("HudGUI/ShopGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickStart()
	{
		if (m_equipedWeapon != null)
		{
			GameObject champObj = (GameObject)Instantiate(m_prefChamp, m_spawnChamp.position, m_spawnChamp.localRotation);
			GameObject prefEnemyBody = Resources.Load<GameObject>("Pref/mon_skin/newchamp_skin");
			
			champObj.name = "Champ";
			
			GameObject enemyBody = Instantiate (prefEnemyBody, Vector3.zero, Quaternion.Euler (0, 0, 0)) as GameObject;
			enemyBody.name = "Body";
			enemyBody.transform.parent = champObj.transform;
			enemyBody.transform.localPosition = Vector3.zero;
			enemyBody.transform.localRotation = prefEnemyBody.transform.rotation;
			
			Champ champ = champObj.GetComponent<Champ>();
			champ.Init();
			m_equipedWeapon.Item.Equip(champ);
			for(int x = 0; x < m_equipedAccessories.Length; ++x)
			{
				if (m_equipedAccessories[x] != null)
				{
					m_equipedAccessories[x].Item.Equip(champ);
				}
			}	
			
			m_spawn.StartWave(m_stage-1, champ);
			
			GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", "Stage:"+m_stage, 0);
			GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", m_equipedWeapon.Item.RefItem.codeName+"_Lv:"+m_equipedWeapon.Item.Level, 0);
			
			//champObj.SetActive(false);

			gameObject.SetActive(false);
		}
	}

	public void OnClickEquip(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, int itemIndex)
	{
		{
			ItemObject selectedItem = Warehouse.Instance.Items[itemIndex];
			
			bool inEquipSlot = m_equipedWeapon == selectedItem;
			if (inEquipSlot == false)
			{
				for(int e = 0; e < m_equipedAccessories.Length; ++e)
				{
					if (m_equipedAccessories[e] == selectedItem)
					{
						inEquipSlot = true;
						break;
					}
				}	
			}
			
			switch(selectedItem.Item.RefItem.type)
			{
			case ItemData.Type.Weapon:
			{
				if (true == inEquipSlot)
				{
					m_equipedWeapon = null;		
					m_weapon.Icon.Image = null;
					SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, itemIndex);
				}
				else
				{
					m_equipedWeapon = selectedItem;
					m_weapon.Icon.Image = selectedItem.ItemIcon;
					SetButtonRole(ButtonRole.Unequip, invSlot, priceGemButton, itemIndex);
				}
			}break;
				
			case ItemData.Type.Accessory:
			case ItemData.Type.Follower:
			{
				if (true == inEquipSlot)
				{
					
					for(int x = 0; x < m_equipedAccessories.Length; ++x)
					{
						if (m_equipedAccessories[x] != null)
						{
							if (m_equipedAccessories[x].Item.Compare(selectedItem.Item))
							{
								m_equipedAccessories[x] = null;
								m_accessories[x].Icon.Image = null;
								SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, itemIndex);
								break;
							}
							
						}
					}					
				}
				else
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
								m_accessories[x].Icon.Image = selectedItem.ItemIcon;
								SetButtonRole(ButtonRole.Unequip, invSlot, priceGemButton, itemIndex);
								break;
							}
						}	
					}
					
				}
			}break;
			}
		}

	}

	public void OnClickLevelup(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, int itemIndex)
	{

		ItemObject selectedItem = Warehouse.Instance.Items[itemIndex];
		if (selectedItem.Item.Level < Const.ItemMaxLevel)
		{
			if (button.TryToPay())
			{
				++selectedItem.Item.Level;

				priceGemButton.m_priceButton.NormalWorth = getItemLevelupWorth(selectedItem);
				priceGemButton.m_gemButton.NormalWorth = getItemLevelupWorth(selectedItem);

				invSlot.ItemDesc = selectedItem.Item.Description();
				GPlusPlatform.Instance.AnalyticsTrackEvent("Weapon", "Levelup", selectedItem.Item.RefItem.codeName + "_Lv:" + selectedItem.Item.Level, 0);
			}
			else
			{
				PopupShop();
			}
		}
	}

	public void OnClickUnlock(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, int itemIndex)
	{
		
		ItemObject selectedItem = Warehouse.Instance.Items[itemIndex];
		if (selectedItem.Item.Lock == true)
		{
			if (button.TryToPay() == true)
			{
				selectedItem.Item.Lock = false;
				SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, itemIndex);

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
			}
			else
			{
				PopupShop();
			}
		}
	}

	float getItemLevelupWorth(ItemObject itemObj)
	{
		return 1f + (itemObj.Item.Level-1);
	}

	float getItemEvolutionWorth(ItemObject itemObj)
	{
		return itemObj.Item.Evolution+1;
	}
}

