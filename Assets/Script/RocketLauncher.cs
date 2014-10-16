using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

	float m_speed = 10f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(m_speed*Time.deltaTime, 0, 0, transform);
	}

	IEnumerator destoryObject()
	{
		m_speed = 0;
		yield return new WaitForSeconds (2);
		DestroyObject(this.gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy")
		{
			DestroyObject(other.gameObject);
			StartCoroutine(destoryObject());
		}
		else if (other.tag == "Wall")
		{
			StartCoroutine(destoryObject());
		}
	}
}
