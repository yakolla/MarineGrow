using UnityEngine;
using System.Collections;

public class Enemy : Creature {

	Vector3			m_targetPos;

	// Use this for initialization
	new void Start () {
		base.Start();
		m_material = transform.Find("Body/mon_a").GetComponent<SkinnedMeshRenderer>().material;

	}
	
	// Update is called once per frame
	void Update () {


		if (AutoAttack() == false)
		{
			m_navAgent.SetDestination(m_targetPos);
		}
		else
		{
			m_navAgent.Stop();

		}

	}

	public void SetTargetPos(Vector3 pos )
	{
		m_targetPos = pos;
	}

}
