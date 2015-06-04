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
	YGUISystem.GUILable		m_score;

	YGUISystem.GUILable		m_deltaGainedGold;
	YGUISystem.GUILable		m_deltaGainedXP;
	YGUISystem.GUILable		m_deltaSurvivalTime;
	YGUISystem.GUILable		m_deltaKilledMobs;
	YGUISystem.GUILable		m_deltaScore;

	void Start () {

		m_admob = GameObject.Find("HudGUI/ADMob").GetComponent<ADMob>();

		m_gainedGold = new YGUISystem.GUILable(transform.Find("Gained Gold/Text").gameObject);
		m_gainedXP = new YGUISystem.GUILable(transform.Find("Gained XP/Text").gameObject);
		m_survivalTime = new YGUISystem.GUILable(transform.Find("Survival Time/Text").gameObject);
		m_killedMobs = new YGUISystem.GUILable(transform.Find("Killed Mobs/Text").gameObject);
		m_score = new YGUISystem.GUILable(transform.Find("Score/Text").gameObject);

		m_deltaGainedGold = new YGUISystem.GUILable(transform.Find("Gained Gold/DeltaText").gameObject);
		m_deltaGainedXP = new YGUISystem.GUILable(transform.Find("Gained XP/DeltaText").gameObject);
		m_deltaSurvivalTime = new YGUISystem.GUILable(transform.Find("Survival Time/DeltaText").gameObject);
		m_deltaKilledMobs = new YGUISystem.GUILable(transform.Find("Killed Mobs/DeltaText").gameObject);
		m_deltaScore = new YGUISystem.GUILable(transform.Find("Score/DeltaText").gameObject);

		m_admob.ShowInterstitial();
		m_admob.ShowBanner(true);

		Warehouse.Instance.NewGameStats.m_score = Warehouse.Instance.NewGameStats.m_gainedXP;
		
		m_gainedGold.Text.text = Warehouse.Instance.NewGameStats.m_gainedGold.ToString();
		m_deltaGainedGold.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_gainedGold, Warehouse.Instance.NewGameStats.m_gainedGold);

		m_gainedXP.Text.text = Warehouse.Instance.NewGameStats.m_gainedXP.ToString();
		m_deltaGainedXP.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_gainedXP, Warehouse.Instance.NewGameStats.m_gainedXP);

		m_survivalTime.Text.text = ((int)Warehouse.Instance.PlayTime).ToString();
		m_deltaSurvivalTime.Text.text = DeltaValue((int)Warehouse.Instance.GameBestStats.m_playTime, (int)Warehouse.Instance.PlayTime);

		m_killedMobs.Text.text = Warehouse.Instance.NewGameStats.m_killedMobs.ToString();
		m_deltaKilledMobs.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_killedMobs, Warehouse.Instance.NewGameStats.m_killedMobs);

		m_score.Text.text = Warehouse.Instance.NewGameStats.m_score.ToString();
		m_deltaScore.Text.text = DeltaValue(Warehouse.Instance.GameBestStats.m_score, Warehouse.Instance.NewGameStats.m_score);

		GPlusPlatform.Instance.ReportScore(Const.TOTAL_SCORE, Warehouse.Instance.NewGameStats.m_score, (bool success) => {
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

}
