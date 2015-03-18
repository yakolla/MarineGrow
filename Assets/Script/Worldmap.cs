using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class Worldmap : MonoBehaviour {
	Rect 		m_statusWindowRect = new Rect(0, 0, Screen.width, Screen.height);
	float 		m_width = Screen.width * (1/5f);
	float 		m_height = Screen.height * (1/8f);
	string		log = "log";
	GameObject	m_selectedMap;

	void OnGUI()
	{
		m_statusWindowRect = GUI.Window ((int)Const.GUI_WindowID.MainMenu, m_statusWindowRect, DisplayStatusWindow, "");	

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			//Application.Quit();
		}
	}
	
	public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.FileName = game.Filename;
			Application.LoadLevel("Basic Dungeon");
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	public void OnSavedGameOpenedForLoading(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			GPlusPlatform.Instance.LoadGame(game, OnSavedGameDataRead);
			Warehouse.Instance.FileName = game.Filename;

		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	public void OnSavedGameOpenedForSaving(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			System.TimeSpan totalPlayingTime = new System.TimeSpan(System.TimeSpan.TicksPerSecond*0);
			Warehouse.Instance.Reset();
			GPlusPlatform.Instance.SaveGame(game, Warehouse.Instance.Serialize(), totalPlayingTime, null, OnSavedGameWritten);
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			Application.LoadLevel("Basic Dungeon");
		} else {
			// handle error
		}
		log = "OnSavedGameWritten:" + status + game;
	}

	public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		log = "OnSavedGameDataRead:" + status;
		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.Deserialize(data);
			Application.LoadLevel("Basic Dungeon");
		} else {
			// handle error
		}
	}

	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;
		GUI.BeginGroup(m_statusWindowRect);
		if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY, size*3, size), "Start"))
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				GPlusPlatform.Instance.Login((bool success) => {
					// handle success or failure
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
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*4, size*3, size), "Achievement"))
		{
			GPlusPlatform.Instance.ShowAchievementsUI();
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*5, size*3, size), "Leaderboard"))
		{
			GPlusPlatform.Instance.ShowLeaderboardUI();
		}
		GUI.TextArea(new Rect(0, startY+size*6, m_statusWindowRect.width, size), log);
		GUI.EndGroup();
	}
}
