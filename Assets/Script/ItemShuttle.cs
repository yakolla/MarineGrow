using UnityEngine;
using System.Collections;

public class ItemShuttle : MonoBehaviour {

	GameObject[]	m_planeSpots;
	Vector3	m_start;
	Vector3	m_goal;
	Vector3	m_dir;

	void Start () {
		m_planeSpots = GameObject.FindGameObjectsWithTag("ItemShuttleSpot");

		if (1 < m_planeSpots.Length)
		{
			int start = Random.Range(0, m_planeSpots.Length);
			int goal = start;

			while(start == goal)
			{
				goal = Random.Range(0, m_planeSpots.Length);
			}

			m_start = m_planeSpots[start].transform.position;
			m_goal = m_planeSpots[goal].transform.position;
			transform.position = m_start;

			float targetHorAngle = Mathf.Atan2(m_goal.z-m_start.z, m_goal.x-m_start.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);


		}

	}

	// Update is called once per frame
	void Update () {

		transform.position = Vector3.MoveTowards(transform.position, m_goal, Time.deltaTime);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("Champ") == 0)
		{
	
		};
	}
}
