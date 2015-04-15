using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropShip : MonoBehaviour {

	Champ	m_champ;

	void Start()
	{

	}

	public void SetChamp(Champ champ)
	{
		m_champ = champ;
	}

	void OnDropChamp()
	{
		m_champ.gameObject.SetActive(true);
	}
}

