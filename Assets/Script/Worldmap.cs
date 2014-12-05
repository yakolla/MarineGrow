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

		if (m_selectedMap == null)
		{
			if(Input.GetMouseButtonUp(0)){
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
				
				if(hitCollider){
					m_selectedMap = hitCollider.gameObject;
				}
				
			}
		}


	}

	void OnGUI()
	{
		if (m_selectedMap != null)
		{
			m_statusWindowRect = GUI.Window (30, m_statusWindowRect, DisplayStatusWindow, "");	
		}

	}

	void DisplayStatusWindow(int windowID)
	{
		int startY = 0;
		int size = (int)m_height;

		if (GUI.Button(new Rect(Screen.width-size, 0, size, size), "X"))
		{
			m_selectedMap = null;
			return;
		}
		
		GUILayout.BeginVertical();

		GUILayout.Label("Desc");
		GUILayout.Label(m_selectedMap.name);

		GUILayout.Label("Rewards");
		GUILayout.SelectionGrid(0, new string[]{"", "", ""}, 3);

		if (GUILayout.Button("GO"))
		{
			Application.LoadLevel(m_selectedMap.name);
		}

		GUILayout.EndVertical();
		
	}
}
