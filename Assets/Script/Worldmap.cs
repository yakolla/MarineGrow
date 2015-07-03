﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;


public class Worldmap : MonoBehaviour {

	string		log = "log";
	GameObject	m_selectedMap;

	void Start()
	{
		Const.HideLoadingGUI();
		Login();
	}

	IEnumerator DelayMessage (string function, float delay)
	{
		
		yield return new WaitForSeconds(delay);
		
		SendMessage(function);
	}

	int m_try = 0;
	void OnOpenSavedGameForLoading(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			GPlusPlatform.Instance.LoadGame(game, OnReadGame);
			Warehouse.Instance.FileName = game.Filename;

		} else {
			if (m_try < 3)
			{
				++m_try;
				log = "OnOpenSavedGameForLoading:" + status + m_try;

				StartCoroutine(DelayMessage("OpenGame", 1f));
				return;
			}

			Const.HideLoadingGUI();
		}


		log = "OnOpenSavedGameForLoading:" + status + game;
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
			//Const.HideLoadingGUI();
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

		}
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(0, 0, 300, 100), log))
		{
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
		}
	}

	public void OnClickStart()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Const.ShowLoadingGUI("Loading...");
			log = "OnClickStart";

			m_try = 0;
			OpenGame();

		}
		else
		{
			Application.LoadLevel("Basic Dungeon");
		}
	}

	public void OpenGame()
	{
		GPlusPlatform.Instance.OpenGame("growingmarine.sav", OnOpenSavedGameForLoading);
	}

	public void OnClickLeaderBoard()
	{
		GPlusPlatform.Instance.ShowLeaderboardUI();
	}

	public void OnClickAchievement()
	{
		GPlusPlatform.Instance.ShowAchievementsUI();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			transform.Find("ExitGUI/Panel").gameObject.SetActive(true);
		}
	}
}
