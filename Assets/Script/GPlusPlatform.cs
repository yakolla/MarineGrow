using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class GPlusPlatform {


	static GPlusPlatform m_ins = null;
	static public GPlusPlatform Instance
	{
		get {
			if (m_ins == null)
			{
				m_ins = new GPlusPlatform();

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

			return m_ins;
		}
	}

	public void Login(System.Action<bool> callback)
	{
		Social.localUser.Authenticate(callback);
	}

	public void OpenGame(string filename, System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
		                                                    ConflictResolutionStrategy.UseLongestPlaytime, callback);
	}

	public void LoadGame(ISavedGameMetadata game, System.Action<SavedGameRequestStatus, byte[]> callback)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, callback);
	}

	public void SaveGame(ISavedGameMetadata game, byte[] savedData, System.TimeSpan totalPlaytime, Texture2D img, System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
		
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		
		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime(totalPlaytime)
				.WithUpdatedDescription("Saved game at " + System.DateTime.Now);

		if (img != null)
		{
			byte[] pngData = img.EncodeToPNG();
			builder = builder.WithUpdatedPngCoverImage(pngData);
		}
		
		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
		
	}

	public void ShowSavedGameBoard(uint maxNumToDisplay, System.Action<SelectUIStatus, ISavedGameMetadata> callback) {
		bool allowCreateNew = true;
		bool allowDelete = true;
		
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ShowSelectSavedGameUI("Select saved game",
		                                      maxNumToDisplay,
		                                      allowCreateNew,
		                                      allowDelete,
		                                      callback);
	}

	public void ShowLeaderboardUI()
	{
		Social.ShowLeaderboardUI();
	}
}
