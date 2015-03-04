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

	GameObject	m_selectedMap;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{
		m_statusWindowRect = GUI.Window ((int)Const.GUI_WindowID.MainMenu, m_statusWindowRect, DisplayStatusWindow, "");	

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			Application.Quit();
		}
	}

	
	void ShowSelectUI() {
		uint maxNumToDisplay = 5;
		bool allowCreateNew = true;
		bool allowDelete = true;
		
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ShowSelectSavedGameUI("Select saved game",
		                                      maxNumToDisplay,
		                                      allowCreateNew,
		                                      allowDelete,
		                                      OnSavedGameSelected);
	}
	
	
	public void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata game) {
		if (status == SelectUIStatus.SavedGameSelected) {
			// handle selected game save
			byte[] data = {0,1};
			System.TimeSpan totalPlayingTime = new System.TimeSpan(1000*60);
			SaveGame(game, data, totalPlayingTime); 

		} else {
			// handle cancel or error
		}

	}

	void OpenSavedGame(string filename) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
		                                                    ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
	}
	
	public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			// handle reading or writing of saved game.
		} else {
			// handle error
		}
	}

	void SaveGame (ISavedGameMetadata game, byte[] savedData, System.TimeSpan totalPlaytime) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		
		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime(totalPlaytime)
				.WithUpdatedDescription("Saved game at " + System.DateTime.Now);

		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
	}
	
	public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			// handle reading or writing of saved game.
		} else {
			// handle error
		}
	}

	void LoadGameData (ISavedGameMetadata game) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
	}
	
	public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		if (status == SavedGameRequestStatus.Success) {
			// handle processing the byte array data
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
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*1, size*3, size), "Google+"))
		{			
			Social.localUser.Authenticate((bool success) => {
				// handle success or failure
				if (success == true)
				{
					PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
						// enables saving game progress.
						.EnableSavedGames()
							.Build();
					
					PlayGamesPlatform.InitializeInstance(config);
					// recommended for debugging:
					PlayGamesPlatform.DebugLogEnabled = true;
					// Activate the Google Play Games platform
					PlayGamesPlatform.Activate();
				}
			});
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*2, size*3, size), "Load"))
		{
			ShowSelectUI();
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*3, size*3, size), "Save"))
		{
			ShowSelectUI();
		}
		else if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*4, size*3, size), "Leaderboard"))
		{
			Social.ShowLeaderboardUI();
		}
		GUI.EndGroup();
	}
}
