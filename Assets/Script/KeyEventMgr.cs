using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class KeyEventMgr : MonoBehaviour {

	GameObject	m_settings;
	GameObject	m_shop;
	GameObject	m_option;
	GameObject	m_goMainTitle;


	void Start()
	{
		m_settings = transform.Find("SettingGUI/Panel").gameObject;
		m_shop = transform.Find("ShopGUI/Panel").gameObject;
		m_option = transform.Find("OptionGUI/Panel").gameObject;
		m_goMainTitle = transform.Find("GoMainTitleGUI/Panel").gameObject;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			if (m_option.activeSelf)
			{
				m_option.SetActive(false);
			}
			else if (m_shop.activeSelf)
			{
			}
			else if (m_settings.activeSelf)
			{
				m_goMainTitle.SetActive(true);
			}
		}
	}
}

