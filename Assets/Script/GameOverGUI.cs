using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

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

		Warehouse.Instance.NewGameStats.m_score = Warehouse.Instance.NewGameStats.m_gainedXP;
		Warehouse.Instance.NewGameStats.m_playTime = Warehouse.Instance.PlayTime;
		
		m_gainedGold.Text.text = Warehouse.Instance.NewGameStats.m_gainedGold.ToString();
		m_deltaGainedGold.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_gainedGold, Warehouse.Instance.NewGameStats.m_gainedGold);
		m_bestGainedGold.Text.text = Warehouse.Instance.GameBestStats.m_gainedGold.ToString();

		m_gainedXP.Text.text = Warehouse.Instance.NewGameStats.m_gainedXP.ToString();
		m_deltaGainedXP.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_gainedXP, Warehouse.Instance.NewGameStats.m_gainedXP);
		m_bestGainedXP.Text.text = Warehouse.Instance.GameBestStats.m_gainedXP.ToString();

		m_survivalTime.Text.text = Warehouse.Instance.NewGameStats.m_playTime.ToString();
		m_deltaSurvivalTime.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_playTime, Warehouse.Instance.NewGameStats.m_playTime);
		m_bestSurvivalTime.Text.text = Warehouse.Instance.GameBestStats.m_playTime.ToString();

		m_killedMobs.Text.text = Warehouse.Instance.NewGameStats.m_killedMobs.ToString();
		m_deltaKilledMobs.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_killedMobs, Warehouse.Instance.NewGameStats.m_killedMobs);
		m_bestKilledMobs.Text.text = Warehouse.Instance.GameBestStats.m_killedMobs.ToString();

		GPlusPlatform.Instance.ReportScore(m_leaderBoards[0], Warehouse.Instance.NewGameStats.m_gainedGold, (bool success) => {
			// handle success or failure
		});

		GPlusPlatform.Instance.ReportScore(m_leaderBoards[1], Warehouse.Instance.NewGameStats.m_gainedXP, (bool success) => {
			// handle success or failure
		});

		System.TimeSpan totalPlayingTime = new System.TimeSpan((long)(System.TimeSpan.TicksPerSecond*Warehouse.Instance.NewGameStats.m_playTime));
		GPlusPlatform.Instance.ReportScore(m_leaderBoards[2],  (long)(totalPlayingTime.TotalSeconds), (bool success) => {
			// handle success or failure
		});

		GPlusPlatform.Instance.ReportScore(m_leaderBoards[3], Warehouse.Instance.NewGameStats.m_killedMobs, (bool success) => {
			// handle success or failure
		});

		if (Application.platform == RuntimePlatform.Android)
		{
			
			Const.SaveGame((SavedGameRequestStatus status, ISavedGameMetadata game) => {
				if (status == SavedGameRequestStatus.Success) {
					// handle reading or writing of saved game.
				} else {
					// handle error
				}
			});
		}
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
