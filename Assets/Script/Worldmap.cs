using UnityEngine;
using System.Collections;

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
		m_statusWindowRect = GUI.Window ((int)GUIConst.WindowID.MainMenu, m_statusWindowRect, DisplayStatusWindow, "");	
	}

	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;
		GUI.BeginGroup(m_statusWindowRect);
		if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY, size*3, size*2), "Start"))
		{
			Application.LoadLevel("Basic Dungeon");
		}
		if (GUI.Button(new Rect(m_statusWindowRect.width/2-(size*3)/2, startY+size*2, size*3, size*2), "Ladder Board"))
		{

		}
		GUI.EndGroup();
	}
}
