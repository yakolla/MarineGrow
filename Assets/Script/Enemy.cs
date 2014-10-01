using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	NavMeshAgent	m_navAgent;
	GameObject		m_champ = null;
	// Use this for initialization
	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_champ)
		{
			Vector3 pos = m_champ.transform.position;
			m_navAgent.SetDestination(pos);
		}

	}

	public void SetChamp(GameObject obj)
	{
		m_champ = obj;
	}
}
