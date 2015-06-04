using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class OptionGUI : MonoBehaviour {

	ADMob					m_admob;


	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		m_admob.ShowBanner(true);
	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnClickOk()
	{
		m_admob.ShowBanner(false);
		gameObject.SetActive(false);
	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		Application.LoadLevel("Worldmap");
	}

	public void OnClickRate()
	{

	}

}
