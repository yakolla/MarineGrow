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

		bool logined = GPlusPlatform.Instance.IsAuthenticated();
		/*m_btnStart.interactable = logined;
		m_btnLeaderBoard.interactable = logined;
		m_btnAchievement.interactable = logined;
		*/
		Login();
	}
	
	void OnOpenSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game) {

		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.FileName = game.Filename;
			Application.LoadLevel("Basic Dungeon");
			Const.HideLoadingGUI();
		} else {

		}

		log = "OnSavedGameOpened:" + status + game;
	}

	int m_try = 0;
	void OnOpenSavedGameForLoading(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			GPlusPlatform.Instance.LoadGame(game, OnReadGame);
			Warehouse.Instance.FileName = game.Filename;

		} else {
			if (m_try < 3)
			{
				string fileName = "growingmarine.sav";
				GPlusPlatform.Instance.OpenGame(fileName, OnOpenSavedGameForLoading);
				++m_try;
				log = "OnSavedGameOpened:" + status + m_try;
				return;
			}

			Const.HideLoadingGUI();
		}


		log = "OnSavedGameOpened:" + status + game;
	}

	void OnOpenSavedGameForSaving(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {

			System.TimeSpan totalPlayingTime = new System.TimeSpan(System.TimeSpan.TicksPerSecond*0);
			Warehouse.Instance.Reset();
			Warehouse.Instance.FileName = game.Filename;
			GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, null, OnWriteGame);
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	void OnWriteGame (SavedGameRequestStatus status, ISavedGameMetadata game) {

		if (status == SavedGameRequestStatus.Success) {
			Application.LoadLevel("Basic Dungeon");
			Const.HideLoadingGUI();
		} else {

		}

		log = "OnSavedGameWritten:" + status + game;
	}

	void OnReadGame (SavedGameRequestStatus status, byte[] data) {
		log = "OnSavedGameDataRead:" + status;

		if (status == SavedGameRequestStatus.Success) {
			if (data.Length > 0)
				Warehouse.Instance.Deserialize(data);

			Application.LoadLevel("Basic Dungeon");
			Const.HideLoadingGUI();
		} else {

		}


	}

	void Login()
	{

		if (Application.platform == RuntimePlatform.Android)
		{
			Const.ShowLoadingGUI("Try to login");

			GPlusPlatform.Instance.Login((bool success) => {
				// handle success or failure
				m_btnLeaderBoard.interactable = success;
				m_btnAchievement.interactable = success;
				m_btnStart.interactable = success;
				
				if (success == true)
				{
					log = "Login success";
					Const.HideLoadingGUI();
				}
				else
				{
					log = "Login failed";
					Login ();
				}
			});
		}
		else
		{
			m_btnStart.interactable = true;
		}
	}
	/*
	public void OnGUI()
	{
		GUI.Button(new Rect(0, 0, 300, 100), log);
	}
*/
	public void OnClickStart()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Const.ShowLoadingGUI("Loading...");
			log = "OnClickStart";

			/*
			GPlusPlatform.Instance.ShowSavedGameBoard(3, (SelectUIStatus status, ISavedGameMetadata game) => {
				if (status == SelectUIStatus.SavedGameSelected) {
					
					string fileName = game.Filename;
					if (fileName.Equals(""))
					{
						fileName = System.DateTime.Now.Ticks.ToString();
						GPlusPlatform.Instance.OpenGame(fileName, OnOpenSavedGameForSaving);
						
					}
					else
					{
						GPlusPlatform.Instance.OpenGame(fileName, OnOpenSavedGameForLoading);
					}
					
					
				} else {
					// handle cancel or error
					Const.HideLoadingGUI();
				}
				log = status.ToString();
			});
				*/

			m_try = 0;
			string fileName = "growingmarine.sav";
			GPlusPlatform.Instance.OpenGame(fileName, OnOpenSavedGameForLoading);

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
