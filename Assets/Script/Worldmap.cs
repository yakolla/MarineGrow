using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class Worldmap : MonoBehaviour {

	string		log = "log";
	GameObject	m_selectedMap;

	Button		m_btnStart;
	Button		m_btnLeaderBoard;
	Button		m_btnAchievement;

	void Start()
	{
		m_btnStart = transform.Find("ButtonStart").GetComponent<Button>();
		m_btnLeaderBoard = transform.Find("ButtonLeaderBoard").GetComponent<Button>();
		m_btnAchievement = transform.Find("ButtonAchievement").GetComponent<Button>();

		m_btnLeaderBoard.interactable = GPlusPlatform.Instance.IsAuthenticated();
		m_btnAchievement.interactable = GPlusPlatform.Instance.IsAuthenticated();
	}
	
	void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.FileName = game.Filename;
			Application.LoadLevel("Basic Dungeon");
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	void OnSavedGameOpenedForLoading(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			GPlusPlatform.Instance.LoadGame(game, OnSavedGameDataRead);
			Warehouse.Instance.FileName = game.Filename;

		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	void OnSavedGameOpenedForSaving(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {

			System.TimeSpan totalPlayingTime = new System.TimeSpan(System.TimeSpan.TicksPerSecond*0);
			Warehouse.Instance.Reset();
			Warehouse.Instance.FileName = game.Filename;
			GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, null, OnSavedGameWritten);
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			Application.LoadLevel("Basic Dungeon");
		} else {
			// handle error
		}
		log = "OnSavedGameWritten:" + status + game;
	}

	void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		log = "OnSavedGameDataRead:" + status;
		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.Deserialize(data);
			Application.LoadLevel("Basic Dungeon");
		} else {
			// handle error
		}
	}

	public void OnClickStart()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			GPlusPlatform.Instance.Login((bool success) => {
				// handle success or failure
				m_btnLeaderBoard.interactable = success;
				m_btnAchievement.interactable = success;

				if (success == true)
				{
					log = "login";
					GPlusPlatform.Instance.ShowSavedGameBoard(3, (SelectUIStatus status, ISavedGameMetadata game) => {
						if (status == SelectUIStatus.SavedGameSelected) {
							
							string fileName = game.Filename;
							if (fileName.Equals(""))
							{
								fileName = System.DateTime.Now.Ticks.ToString();
								GPlusPlatform.Instance.OpenGame(fileName, OnSavedGameOpenedForSaving);
								
							}
							else
							{
								GPlusPlatform.Instance.OpenGame(fileName, OnSavedGameOpenedForLoading);
							}
							
							
						} else {
							// handle cancel or error
						}
					});
				}
			});
		}
		else
		{
			Application.LoadLevel("Basic Dungeon");
		}
	}

	public void OnClickLeaderBoard()
	{
		GPlusPlatform.Instance.ShowLeaderboardUI();
	}

	public void OnClickAchievement()
	{
		GPlusPlatform.Instance.ShowAchievementsUI();
	}
}
