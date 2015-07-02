using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class ChampSettingGUI : MonoBehaviour {


	GameObject	m_inventoryObj;
	YGUISystem.GUIImageStatic	m_gold;
	YGUISystem.GUIImageStatic	m_goldMedal;
	YGUISystem.GUIImageStatic	m_gem;
	YGUISystem.GUIButton	m_weapon;
	YGUISystem.GUILockButton[]	m_accessories = new YGUISystem.GUILockButton[Const.AccessoriesSlots];
	YGUISystem.GUIButton	m_start;

	[SerializeField]
	Transform		m_spawnChamp;

	[SerializeField]
	bool		m_cheat = true;

	public class EquippedContext
	{
		public ItemObject		m_itemObject;
		public GUIInventorySlot m_inventorySlot;
	}
	EquippedContext		m_equipedWeapon = new EquippedContext();
	EquippedContext[]	m_equipedAccessories = new EquippedContext[Const.AccessoriesSlots];


	int			m_stage = 1;

	Spawn		m_spawn;

	string		log;


	void Start()
	{
		System.GC.Collect();

		Const.CHEAT_MODE = m_cheat;

		m_spawn = GameObject.Find("Dungeon/Spawn").GetComponent<Spawn>();
		GPlusPlatform.Instance.InitAnalytics(GameObject.Find("GAv3").GetComponent<GoogleAnalyticsV3>());
		GPlusPlatform.Instance.AnalyticsTrackScreen("SettingGUI");

		if (m_cheat == true)
		{
#if UNITY_EDITOR
			if (Warehouse.Instance.Items.Count == 0)
			{

				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGunRefItemId));

				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampFiregunRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampLightningLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGuidedRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampBoomerangLauncherRefItemId));
				/*
				foreach(KeyValuePair<int, RefItem> keyPair in RefData.Instance.RefItems)
				{
					if (Warehouse.Instance.FindItem(keyPair.Key) != null)
						continue;
					
					switch(keyPair.Value.type)
					{
					case ItemData.Type.Weapon:
						ItemWeaponData weaponData = new ItemWeaponData(keyPair.Key);
						Warehouse.Instance.PushItem(weaponData);
						break;
					}
				}
*/

				Warehouse.Instance.PushItem(new ItemAccessoryData(Const.BootsRefItemId));

				Warehouse.Instance.Gold.Item.Count = 100000;
				Warehouse.Instance.GoldMedal.Item.Count = 1000;
				Warehouse.Instance.Gem.Item.Count = 12000;

				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					ItemFollowerData followerData = new ItemFollowerData(follower.id);
					Warehouse.Instance.PushItem(followerData);
				}

				Warehouse.Instance.PushItem(new ItemCheatData());
			}
#endif
		}
		else
		{
			//Load();

			if (Warehouse.Instance.Items.Count == 0)
			{
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGunRefItemId));

				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampFiregunRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampLightningLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampGuidedRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampRocketLauncherRefItemId));
				Warehouse.Instance.PushItem(new ItemWeaponData(Const.ChampBoomerangLauncherRefItemId));

				foreach(RefMob follower in RefData.Instance.RefFollowerMobs)
				{
					ItemFollowerData followerData = new ItemFollowerData(follower.id);
					Warehouse.Instance.PushItem(followerData);
				}

				Warehouse.Instance.PushItem(new ItemCheatData());
			}
			
			byte[] data = Warehouse.Instance.Serialize();
			Warehouse.Instance.Deserialize(data);

		}

		for(int i = 0; i < m_equipedAccessories.Length; ++i)
		{
			m_equipedAccessories[i] = new EquippedContext();
		}

		m_weapon = new YGUISystem.GUIButton(transform.Find("WeaponButton").gameObject, ()=>{return true;});
		for(int i = 0; i < m_accessories.Length; ++i)
		{
			m_accessories[i] = new YGUISystem.GUILockButton(transform.Find("AccessoryButton" + i).gameObject, ()=>{return true;});

			if (i < Const.HalfAccessoriesSlots)
				m_accessories[i].Lock = false;
		}

		m_gold = new YGUISystem.GUIImageStatic(transform.Find("GoldImage").gameObject, Warehouse.Instance.Gold.ItemIcon);
		m_goldMedal = new YGUISystem.GUIImageStatic(transform.Find("GoldMedalImage").gameObject, Warehouse.Instance.GoldMedal.ItemIcon);
		m_gem = new YGUISystem.GUIImageStatic(transform.Find("GemImage").gameObject, Warehouse.Instance.Gem.ItemIcon);
		m_start = new YGUISystem.GUIButton(transform.Find("StartButton").gameObject, ()=>{return m_equipedWeapon != null;});

		RectTransform rectScrollView = transform.Find("InvScrollView").gameObject.GetComponent<RectTransform>();
		m_inventoryObj = transform.Find("InvScrollView/Contents").gameObject;
		RectTransform rectInventoryObj = m_inventoryObj.GetComponent<RectTransform>();
		Vector2 rectContents = new Vector2(	rectInventoryObj.rect.width, 0);

		GameObject prefGUIInventorySlot = Resources.Load<GameObject>("Pref/GUIInventorySlot");
		RectTransform	rectGUIInventorySlot = prefGUIInventorySlot.GetComponent<RectTransform>();

		int itemIndex = 0;
		int maxCount = Warehouse.Instance.Items.Count;
		int equipItemIndex = 0;
		for(itemIndex = 0; itemIndex < maxCount; ++itemIndex)
		{
			ItemObject item = Warehouse.Instance.Items[itemIndex];

			GameObject obj = Instantiate(prefGUIInventorySlot) as GameObject;
			GUIInventorySlot invSlot = obj.GetComponent<GUIInventorySlot>();

			obj.transform.parent = m_inventoryObj.transform;
			obj.transform.localScale = prefGUIInventorySlot.transform.localScale;
			obj.transform.localPosition = new Vector3(0f, rectGUIInventorySlot.rect.height/2*(maxCount-1)-rectGUIInventorySlot.rect.height*itemIndex, 0);

			invSlot.Init(item.ItemIcon, item.Item.Description());
			invSlot.PriceButton0.EnableChecker = ()=>{return false;};
			invSlot.PriceButton1.EnableChecker = ()=>{return false;};

			int capturedItemIndex = itemIndex;


			switch(item.Item.RefItem.type)
			{
			case ItemData.Type.Weapon:
			case ItemData.Type.Accessory:
			case ItemData.Type.Follower:
			case ItemData.Type.Skill:
			case ItemData.Type.Cheat:
				if (item.Item.Lock == true)
				{
					if (item.Item.RefItem.unlock != null)
					{
						SetButtonRole(ButtonRole.Unlock, invSlot, invSlot.PriceButton0, itemIndex);
					}
				}
				else
				{
					SetButtonRole(ButtonRole.Equip, invSlot, invSlot.PriceButton0, itemIndex);
				}

				if (item.Item.RefItem.levelup != null)
				{
					SetButtonRole(ButtonRole.Levelup, invSlot, invSlot.PriceButton1, itemIndex);
				}
				break;
			}

			invSlot.Update();

			if (Warehouse.Instance.ChampEquipItems.m_weaponRefItemId == item.Item.RefItemID)
			{
				equipItemIndex = itemIndex;
				OnClickEquip(invSlot, invSlot.PriceButton0, invSlot.PriceButton0.m_priceButton, itemIndex);
			}
			else
			{
				for(int x = 0; x < m_equipedAccessories.Length; ++x)
				{
					if (Warehouse.Instance.ChampEquipItems.m_accessoryRefItemId[x] == item.Item.RefItemID)
					{
						OnClickEquip(invSlot, invSlot.PriceButton0, invSlot.PriceButton0.m_priceButton, itemIndex);
						break;
					}
				}
			}
		}
		rectContents.y = rectGUIInventorySlot.rect.height*itemIndex;
		rectInventoryObj.sizeDelta = rectContents;
		//rectInventoryObj.position = new Vector3(rectInventoryObj.position.x, -(rectContents.y/2-rectScrollView.rect.height/2), rectInventoryObj.position.z);
		rectInventoryObj.localPosition = new Vector3(0, -(rectContents.y/2-rectScrollView.rect.height/2-rectGUIInventorySlot.rect.height*equipItemIndex), 0);
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
			priceGemButton.EnableChecker = ()=>{return item.Item.RefItem.type != ItemData.Type.Cheat;};

				priceGemButton.SetPrices(null, null);
				priceGemButton.AddListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );

				if (item.Item.RefItem.type == ItemData.Type.Follower)
					priceGemButton.SetLable("Follow");
				else
					priceGemButton.SetLable("Equip");

				invSlot.SetListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex));
			}
			break;
		case ButtonRole.Unequip:
			{
			priceGemButton.EnableChecker = ()=>{return true;};

				priceGemButton.SetPrices(null, null);
				priceGemButton.AddListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );

			if (item.Item.RefItem.type == ItemData.Type.Follower)
				priceGemButton.SetLable("Unfollow");
			else
				priceGemButton.SetLable("Unequip");
				
				invSlot.SetListener(() => OnClickEquip(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex));
			}
			break;

		case ButtonRole.Levelup:
			{

				priceGemButton.EnableChecker = ()=>{return item.Item.RefItem.levelup.conds != null && item.Item.Lock == false && item.Item.Level < Const.MaxItemLevel;};
				
				priceGemButton.SetPrices(item.Item.RefItem.levelup.conds, item.Item.RefItem.levelup.else_conds);
				priceGemButton.m_priceButton.NormalWorth = Const.GetItemLevelupWorth(item.Item.Level, item.Item.RefItem.levelup);
				priceGemButton.m_gemButton.NormalWorth = Const.GetItemLevelupWorth(item.Item.Level, item.Item.RefItem.levelup);

				priceGemButton.AddListener(() => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_priceButton, itemIndex), () => OnClickLevelup(invSlot, priceGemButton, priceGemButton.m_gemButton, itemIndex) );
					priceGemButton.SetLable("Levelup");
				}
			break;

		case ButtonRole.Unlock:
			{
			priceGemButton.EnableChecker = ()=>{return item.Item.RefItem.unlock.conds != null;};

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

	void Update()
	{
		m_gold.Lable.Text.text = Warehouse.Instance.Gold.Item.Count.ToString();
		m_goldMedal.Lable.Text.text = Warehouse.Instance.GoldMedal.Item.Count.ToString();
		m_gem.Lable.Text.text = Warehouse.Instance.Gem.Item.Count.ToString();
		m_start.Update();


	}

	public void StartSpinButton(YGUISystem.GUIButton button)
	{
		button.Button.animator.SetBool("Spin", true);
		button.Button.audio.Play();
	}

	void PopupShop()
	{
		GameObject.Find("HudGUI/ShopGUI").transform.Find("Panel").gameObject.SetActive(true);
	}

	public void OnClickStart()
	{
		if (m_equipedWeapon.m_itemObject == null)
			return;


		GameObject champObj = Creature.InstanceCreature(Resources.Load<GameObject>("Pref/Champ"), Resources.Load<GameObject>("Pref/mon_skin/" + RefData.Instance.RefChamp.prefBody), m_spawnChamp.position, m_spawnChamp.localRotation);	
		champObj.name = "Champ";		

		Champ champ = champObj.GetComponent<Champ>();
		champ.Init(RefData.Instance.RefChamp, 1);

		Warehouse.Instance.ChampEquipItems.m_weaponRefItemId = m_equipedWeapon.m_itemObject.Item.RefItemID;

		m_equipedWeapon.m_itemObject.Item.Equip(champ);
		for(int x = 0; x < m_equipedAccessories.Length; ++x)
		{
			if (m_equipedAccessories[x].m_itemObject != null)
			{
				m_equipedAccessories[x].m_itemObject.Item.Equip(champ);
				champ.AccessoryItems[x] = m_equipedAccessories[x].m_itemObject;
				Warehouse.Instance.ChampEquipItems.m_accessoryRefItemId[x] = m_equipedAccessories[x].m_itemObject.Item.RefItemID;
			}
		}	
		
		m_spawn.StartWave(m_stage-1, champ);
		
		GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", "Stage:"+m_stage, 0);
		GPlusPlatform.Instance.AnalyticsTrackEvent("Start", "Setting", m_equipedWeapon.m_itemObject.Item.RefItem.codeName+"_Lv:"+m_equipedWeapon.m_itemObject.Item.Level, 0);
		
		champObj.SetActive(false);

		gameObject.SetActive(false);

	}

	public void OnClickEquip(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, int itemIndex)
	{

		ItemObject selectedItem = Warehouse.Instance.Items[itemIndex];
		
		bool inEquipSlot = m_equipedWeapon.m_itemObject == selectedItem;
		if (inEquipSlot == false)
		{
			for(int e = 0; e < m_equipedAccessories.Length; ++e)
			{
				if (m_equipedAccessories[e].m_itemObject == selectedItem)
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
				m_equipedWeapon.m_itemObject = null;
				m_equipedWeapon.m_inventorySlot = null;
				m_weapon.Icon.Image = null;
				invSlot.Check(false);
				SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, itemIndex);
			}
			else
			{
				if (m_equipedWeapon.m_inventorySlot != null)
				{
					m_equipedWeapon.m_inventorySlot.Check(false);
				}
				m_equipedWeapon.m_itemObject = selectedItem;
				m_equipedWeapon.m_inventorySlot = invSlot;
				m_weapon.Icon.Image = selectedItem.ItemIcon;
				invSlot.Check(true);
				SetButtonRole(ButtonRole.Unequip, invSlot, priceGemButton, itemIndex);
			}
		}break;
			
		case ItemData.Type.Accessory:
		case ItemData.Type.Follower:
		case ItemData.Type.Skill:
		{
			if (true == inEquipSlot)
			{
				
				for(int x = 0; x < Cheat.HowManyAccessorySlot; ++x)
				{
					if (m_equipedAccessories[x].m_itemObject != null)
					{
						if (m_equipedAccessories[x].m_itemObject.Item.Compare(selectedItem.Item))
						{
							m_equipedAccessories[x].m_itemObject = null;
							m_equipedAccessories[x].m_inventorySlot = null;
							m_accessories[x].Icon.Image = null;
							invSlot.Check(false);
							SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, itemIndex);
							break;
						}
						
					}
				}					
			}
			else
			{
				bool aleadyExists = false;
				for(int x = 0; x < Cheat.HowManyAccessorySlot; ++x)
				{
					if (m_equipedAccessories[x].m_itemObject == selectedItem)
					{
						aleadyExists = true;
						break;
					}
				}	
				
				if (aleadyExists == false)
				{
					for(int x = 0; x < Cheat.HowManyAccessorySlot; ++x)
					{
						if (m_equipedAccessories[x].m_itemObject == null)
						{
							m_equipedAccessories[x].m_itemObject = selectedItem;
							m_equipedAccessories[x].m_inventorySlot = invSlot;
							m_accessories[x].Icon.Image = selectedItem.ItemIcon;
							invSlot.Check(true);
							SetButtonRole(ButtonRole.Unequip, invSlot, priceGemButton, itemIndex);
							break;
						}
					}	
				}
				
			}
		}break;
		}
	}

	void UpdateAccessorySlots()
	{
		if (Cheat.HowManyAccessorySlot == Const.HalfAccessoriesSlots)
			return;

		for(int i = Const.HalfAccessoriesSlots; i < m_accessories.Length; ++i)
		{
			m_accessories[i].Lock = false;
		}
	}

	public void OnClickLevelup(GUIInventorySlot invSlot, GUIInventorySlot.GUIPriceGemButton priceGemButton, YGUISystem.GUIPriceButton button, int itemIndex)
	{

		ItemObject selectedItem = Warehouse.Instance.Items[itemIndex];
		if (selectedItem.Item.Level < Const.MaxItemLevel)
		{
			if (button.TryToPay())
			{
				StartSpinButton(priceGemButton.m_priceButton.GUIImageButton);
				++selectedItem.Item.Level;

				if (selectedItem.Item.Level == Const.MaxItemLevel)
				{
					priceGemButton.SetPrices(null, null);

					switch(selectedItem.Item.RefItem.id)
					{
					case Const.ChampGunRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_GUN, 100, (bool success) => {
						});  
						break;
					case Const.ChampFiregunRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_FIREGUN, 100, (bool success) => {
						});  
						break;
					case Const.ChampLightningLauncherRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_LIGHTNING, 100, (bool success) => {
						});  
						break;
					case Const.ChampRocketLauncherRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_ROCKET, 100, (bool success) => {
						});  
						break;
					case Const.ChampGuidedRocketLauncherRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_GUIDEDROCKET, 100, (bool success) => {
						});  
						break;						
					case Const.ChampBoomerangLauncherRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_BOOMERANG, 100, (bool success) => {
						});  
						break;
					case Const.EngineeringBayRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_ENGINEERING, 100, (bool success) => {
						});  
						break;
					case Const.GunMarineRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_GUNMARINE, 100, (bool success) => {
						});  
						break;
					case Const.FireMarineRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_FIREMARINE, 100, (bool success) => {
						});  
						break;
					case Const.RocketMarineRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_ROCKETMARINE, 100, (bool success) => {
						});  
						break;
					case Const.GuidedRocketMarineRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_GUIDEDROCKETMARINE, 100, (bool success) => {
						});  
						break;
					case Const.BoomerangMarineRefItemId:
						GPlusPlatform.Instance.ReportProgress(
							Const.ACH_MAX_LEVEL_BOOMERANGMARINE, 100, (bool success) => {
						});  
						break;
					}
				}

				priceGemButton.m_priceButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);
				priceGemButton.m_gemButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);

				invSlot.ItemDesc = selectedItem.Item.Description();

				UpdateAccessorySlots();

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
				selectedItem.Item.Level = 1;

				invSlot.ItemDesc = selectedItem.Item.Description();

				SetButtonRole(ButtonRole.Equip, invSlot, priceGemButton, itemIndex);


				invSlot.PriceButton1.m_priceButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);
				invSlot.PriceButton1.m_gemButton.NormalWorth = Const.GetItemLevelupWorth(selectedItem.Item.Level, selectedItem.Item.RefItem.levelup);


				switch(selectedItem.Item.RefItem.id)
				{
				case Const.ChampFiregunRefItemId:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_FIREGUN, 100, (bool success) => {
					});  
					break;
				case Const.ChampLightningLauncherRefItemId:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_LIGHTNINGLAUNCHER, 100, (bool success) => {
					});  
					break;
				case Const.ChampRocketLauncherRefItemId:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_ROCKETLAUNCHER, 100, (bool success) => {
					});  
					break;
				case Const.ChampGuidedRocketLauncherRefItemId:
					GPlusPlatform.Instance.ReportProgress(
						Const.ACH_UNLOCKED_THE_GUIDEDROCKETLAUNCHER, 100, (bool success) => {
					});  
					break;
				
				case Const.ChampBoomerangLauncherRefItemId:
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


}

