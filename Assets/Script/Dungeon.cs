using UnityEngine;
using System.Collections;

public class Dungeon : MonoBehaviour {

	[SerializeField]
	int				m_dungeonId;

	RefWorldMap		m_refWorldMap;


	// Use this for initialization
	void Start () {		

		
		m_refWorldMap = RefData.Instance.RefWorldMaps[m_dungeonId];


	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator delayLoadLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("Worldmap");
	}
	
	public void DelayLoadLevel(float delay)
	{
		StartCoroutine(delayLoadLevel(delay));
	}



	public int DungeonId
	{
		get{return m_dungeonId;}
	}
}
