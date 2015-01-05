using UnityEngine;
using System.Collections;

public class Area : MonoBehaviour {

	Spawn	m_spawn;
	new void Start () {
		
		m_spawn = transform.parent.GetComponent<Spawn>();
	}

	// Update is called once per frame
	void Update () {


	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.CompareTo("Champ") == 0)
		{
			m_spawn.SetAreaInChamp(transform);
		};

	}
}
