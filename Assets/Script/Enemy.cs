using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	NavMeshAgent	m_navAgent;
	Vector3			m_targetPos;
	// Use this for initialization
	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		m_navAgent.SetDestination(m_targetPos);
	}

	public void SetTargetPos(Vector3 pos )
	{
		m_targetPos = pos;
	}
}
