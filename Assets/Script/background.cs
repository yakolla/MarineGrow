using UnityEngine;
using System.Collections;

public class background : MonoBehaviour {

	GameObject	m_prefItemBox;
	// Use this for initialization
	void Start () {
		m_prefItemBox = Resources.Load<GameObject>("Pref/ItemBox");
	}

	// Update is called once per frame
	void Update () {

	}

	public void SpawnItemBox(Vector3 pos)
	{
		Instantiate(m_prefItemBox, pos, Quaternion.Euler(0f, 0f, 0f));

	}
}
