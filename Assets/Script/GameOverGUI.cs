using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOverGUI : MonoBehaviour {

	Champ		m_champ;

	YGUISystem.GUIButton	m_reStartButton;
	YGUISystem.GUIButton	m_titleButton;

	void Start () {

		m_reStartButton = new YGUISystem.GUIButton(transform.Find("RestartButton").gameObject, ()=>{return true;});
		m_titleButton = new YGUISystem.GUIButton(transform.Find("TitleButton").gameObject, ()=>{return true;});
	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowInterstitial();
		GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>().ShowBanner(true);
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnClickRestart()
	{
		Application.LoadLevel("Basic Dungeon");
	}

	public void OnClickTitle()
	{
		Application.LoadLevel("Worldmap");
	}

}
