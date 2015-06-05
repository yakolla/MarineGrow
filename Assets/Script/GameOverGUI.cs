using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameOverGUI : MonoBehaviour {

	ADMob					m_admob;

	YGUISystem.GUILable		m_gainedGold;
	YGUISystem.GUILable		m_gainedXP;
	YGUISystem.GUILable		m_survivalTime;
	YGUISystem.GUILable		m_killedMobs;

	YGUISystem.GUILable		m_deltaGainedGold;
	YGUISystem.GUILable		m_deltaGainedXP;
	YGUISystem.GUILable		m_deltaSurvivalTime;
	YGUISystem.GUILable		m_deltaKilledMobs;

	YGUISystem.GUILable		m_bestGainedGold;
	YGUISystem.GUILable		m_bestGainedXP;
	YGUISystem.GUILable		m_bestSurvivalTime;
	YGUISystem.GUILable		m_bestKilledMobs;

	string[]				m_leaderBoards = {Const.LEADERBOARD_GAINED_GOLD, Const.LEADERBOARD_GAINED_XP, Const.LEADERBOARD_SURVIVAL_TIME, Const.LEADERBOARD_KILLED_MOBS};

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();



		m_gainedGold = new YGUISystem.GUILable(transform.Find("Gained Gold/Text").gameObject);
		m_gainedXP = new YGUISystem.GUILable(transform.Find("Gained XP/Text").gameObject);
		m_survivalTime = new YGUISystem.GUILable(transform.Find("Survival Time/Text").gameObject);
		m_killedMobs = new YGUISystem.GUILable(transform.Find("Killed Mobs/Text").gameObject);


		m_deltaGainedGold = new YGUISystem.GUILable(transform.Find("Gained Gold/DeltaText").gameObject);
		m_deltaGainedXP = new YGUISystem.GUILable(transform.Find("Gained XP/DeltaText").gameObject);
		m_deltaSurvivalTime = new YGUISystem.GUILable(transform.Find("Survival Time/DeltaText").gameObject);
		m_deltaKilledMobs = new YGUISystem.GUILable(transform.Find("Killed Mobs/DeltaText").gameObject);


		m_bestGainedGold = new YGUISystem.GUILable(transform.Find("Gained Gold/BestText").gameObject);
		m_bestGainedXP = new YGUISystem.GUILable(transform.Find("Gained XP/BestText").gameObject);
		m_bestSurvivalTime = new YGUISystem.GUILable(transform.Find("Survival Time/BestText").gameObject);
		m_bestKilledMobs = new YGUISystem.GUILable(transform.Find("Killed Mobs/BestText").gameObject);


		m_admob.ShowInterstitial();
		m_admob.ShowBanner(true);


		
		m_gainedGold.Text.text = Warehouse.Instance.NewGameStats.GainedGold.ToString();
		m_deltaGainedGold.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.GainedGold, Warehouse.Instance.NewGameStats.GainedGold);
		m_bestGainedGold.Text.text = Warehouse.Instance.GameBestStats.GainedGold.ToString();

		m_gainedXP.Text.text = Warehouse.Instance.NewGameStats.GainedXP.ToString();
		m_deltaGainedXP.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.GainedXP, Warehouse.Instance.NewGameStats.GainedXP);
		m_bestGainedXP.Text.text = Warehouse.Instance.GameBestStats.GainedXP.ToString();

		m_survivalTime.Text.text = Warehouse.Instance.NewGameStats.SurvivalTime.ToString();
		m_deltaSurvivalTime.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.SurvivalTime, Warehouse.Instance.NewGameStats.SurvivalTime);
		m_bestSurvivalTime.Text.text = Warehouse.Instance.GameBestStats.SurvivalTime.ToString();

		m_killedMobs.Text.text = Warehouse.Instance.NewGameStats.KilledMobs.ToString();
		m_deltaKilledMobs.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.KilledMobs, Warehouse.Instance.NewGameStats.KilledMobs);
		m_bestKilledMobs.Text.text = Warehouse.Instance.GameBestStats.KilledMobs.ToString();


	}

	string DeltaValue(long src, long dest)
	{
		if (src > dest)
			return "<color=red>" + (dest-src).ToString() + "</color>";

		if (src == dest)
			return " 0";

		return "<color=yellow>" + "+" + (dest-src).ToString() + "</color>";
	}

	string DeltaValue(float src, float dest)
	{
		if (src > dest)
			return "<color=red>" + (dest-src).ToString() + "</color>";
		
		if (src == dest)
			return "= 0";
		
		return "<color=yellow>" + "+" + (dest-src).ToString() + "</color>";
	}

	void OnEnable() {
		TimeEffector.Instance.StopTime();
	}

	void OnDisable() {
		TimeEffector.Instance.StartTime();
	}

	public void OnClickRestart()
	{
		m_admob.ShowBanner(false);
		Application.LoadLevel("Basic Dungeon");
	}

	public void OnClickTitle()
	{
		m_admob.ShowBanner(false);
		Application.LoadLevel("Worldmap");
	}

	public void OnClickLeaderBoard(int slot)
	{
		GPlusPlatform.Instance.ShowLeaderboardUI(m_leaderBoards[slot]);
	}

}
