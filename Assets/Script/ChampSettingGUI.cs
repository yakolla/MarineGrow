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


	ItemObject		m_equipedWeapon = null;
	ItemObject[]	m_aquipedAccessory = new ItemObject[EQUIP_ACCESSORY_SLOT_MAX];

	ItemObject		m_latestSelected = null;

	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;

	[SerializeField]
	GUISkin		m_guiSkin = null;

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);

	void Start () {

		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);


		Warehouse.Instance().PushItem(new ItemWeapon("Pref/Firegun"));
		Warehouse.Instance().PushItem(new ItemWeapon("Pref/Gun"));
		Warehouse.Instance().PushItem(new ItemWeapon("Pref/LightningBoltLauncher"));
		Warehouse.Instance().PushItem(new ItemWeapon("Pref/GrenadeLauncher"));
		Warehouse.Instance().PushItem(new ItemWeapon("Pref/RocketLauncher"));

		Save();
		//Load();
	}

	void OnEnable() {
		Time.timeScale = 0;
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

		Warehouse.Instance().Save(bf, file);

		file.Close();
	}

	public void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);

			Warehouse.Instance().Load(bf, file);

			file.Close();
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
			DestroyObject(gameObject);
			return;
		}

		GUI.Label(new Rect(0, startY+(size*0), size, size), "Equip Weapon");
		if (GUI.Button(new Rect(0, startY+(size*1), size, size), m_equipedWeapon != null ? m_equipedWeapon.ItemIcon : null))
		{
			m_equipedWeapon = null;
		}

		GUI.Label(new Rect(size*2, startY+(size*0), size, size), "Equip Accessory");

		for(int x = 0; x < EQUIP_ACCESSORY_SLOT_MAX; x++)
		{
			if (GUI.Button(new Rect(size*(2+x), startY+(size*1), size, size), ""))
			{

			}
		}



		GUI.Label(new Rect(0, startY+(size*2), size, size), "Items");

		for(int y = 0; y < INVEN_SLOT_ROWS; y++)
		{
			for(int x = 0; x < INVEN_SLOT_COLS; x++)
			{
				int id = x+y*INVEN_SLOT_COLS;
				ItemObject item = null;
				if (id < Warehouse.Instance().Items.Count)
					item = Warehouse.Instance().Items[id];				

				if (GUI.Button(new Rect(size*x, startY+(size*(3+y)), size, size), item != null ? item.ItemIcon : null))
				{
					if (item != null && item.Item.ItemType == Item.Type.Weapon)
					{
						m_equipedWeapon = item;
						m_latestSelected = item;
					}
				}
			}
		}


		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*3), size, size), "Desc");
		if (m_latestSelected != null)
		{
			GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*4), size*3, size*3), m_latestSelected.Item.Description());
		}


	}
}
