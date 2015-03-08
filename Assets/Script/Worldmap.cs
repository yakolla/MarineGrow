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
	ISavedGameMetadata m_openGame = null;
	// Use this for initialization
	void Start () {

	}
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	// Update is called once per frame
	void Update () {

	}

	void OnEnable()
	{
		enabled = true;
	}

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
			m_openGame = game;
		} else {
			// handle error
		}
		log = "OnSavedGameOpened:" + status + game;
	}

	public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			// handle reading or writing of saved game.
		} else {
			// handle error
		}
		log = "OnSavedGameWritten:" + status + game;
	}

	public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		log = "OnSavedGameDataRead:" + status;
		if (status == SavedGameRequestStatus.Success) {
			Warehouse.Instance.Deserialize(data);
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
			Application.LoadLevel("Basic Dungeon");
			enabled = false;
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*1, size*3, size), "Google+"))
		{			
			GPlusPlatform.Instance.Login((bool success) => {
				// handle success or failure
				if (success == true)
				{
					log = "login";
				}
			});
			log = "xx";
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*2, size*3, size), "Select a Slot"))
		{
			GPlusPlatform.Instance.ShowSavedGameBoard(3, (SelectUIStatus status, ISavedGameMetadata game) => {
				if (status == SelectUIStatus.SavedGameSelected) {

					string fileName = game.Filename;
					if (fileName.Equals(""))
					{
						GPlusPlatform.Instance.OpenGame(System.DateTime.Now.Ticks.ToString(), OnSavedGameOpened);
					}
					else
					{
						GPlusPlatform.Instance.OpenGame(fileName, OnSavedGameOpened);
					}

				} else {
					// handle cancel or error
				}
			});
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*3, size*3, size), "Load"))
		{
			GPlusPlatform.Instance.LoadGame(m_openGame, OnSavedGameDataRead);
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*4, size*3, size), "Save"))
		{
			System.TimeSpan totalPlayingTime = new System.TimeSpan(System.TimeSpan.TicksPerSecond*6);

			GPlusPlatform.Instance.SaveGame(m_openGame, Warehouse.Instance.Serialize(), totalPlayingTime, null, OnSavedGameWritten);

		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*5, size*3, size), "Leaderboard"))
		{
			GPlusPlatform.Instance.ShowLeaderboardUI();
		}
		GUI.TextArea(new Rect(0, startY+size*6, m_statusWindowRect.width, size), log);
		GUI.EndGroup();
	}
}
