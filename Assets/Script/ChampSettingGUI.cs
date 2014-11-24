using UnityEngine;
using System.Collections;

public class ChampSettingGUI : MonoBehaviour {

	const int INVEN_SLOT_COLS = 4;
	const int INVEN_SLOT_ROWS = 4;
	const int EQUIP_SLOT_MAX = 4;

	[SerializeField]
	GameObject		m_prefChamp;

	[SerializeField]
	ItemBox[]	m_items;

	ItemBox		m_equipedWeapon;
	ItemBox[]	m_aquipedAccessory = new ItemBox[EQUIP_SLOT_MAX];

	ItemBox		m_latestSelected;

	Rect 		m_statusWindowRect;
	Rect 		m_skillWindowRect;

	[SerializeField]
	GUISkin		m_guiSkin;

	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);

	void Start () {

		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
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
	

	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;

		if (GUI.Button(new Rect(Screen.width-size, 0, size, size), "X") && m_equipedWeapon)
		{
			GameObject champObj = (GameObject)Instantiate(m_prefChamp, m_prefChamp.transform.position, m_prefChamp.transform.localRotation);
			m_equipedWeapon.Use(champObj.GetComponent<Creature>());
			DestroyObject(gameObject);
			return;
		}

		GUI.Label(new Rect(0, startY+(size*0), size, size), "Equip Weapon");
		if (GUI.Button(new Rect(0, startY+(size*1), size, size), m_equipedWeapon ? m_equipedWeapon.ItemIcon : null))
		{
			m_equipedWeapon = null;
		}

		GUI.Label(new Rect(size*2, startY+(size*0), size, size), "Equip Accessory");

		for(int x = 0; x < EQUIP_SLOT_MAX; x++)
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
				ItemBox itemBox = null;
				if (id < m_items.Length)
					itemBox = m_items[id];				

				if (GUI.Button(new Rect(size*x, startY+(size*(3+y)), size, size), itemBox ? itemBox.ItemIcon : null))
				{
					if (itemBox != null && itemBox.ItemType == ItemBox.Type.Weapon)
					{
						m_equipedWeapon = itemBox;
						m_latestSelected = itemBox;
					}
				}
			}
		}


		GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*3), size, size), "Desc");
		if (m_latestSelected)
		{
			GUI.Label(new Rect(size*(INVEN_SLOT_COLS+1), startY+(size*4), size*3, size*3), m_latestSelected.Description());
		}


	}
}
