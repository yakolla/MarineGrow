using UnityEngine;
using System.Collections;

public class RocketLauncherBullet : Bullet {

	float m_speed = 10f;
	public float	m_damage = 4;
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
		if (other.tag.CompareTo("Enemy") == 0)
		{
			other.gameObject.GetComponent<Enemy>().TakeDamage(m_damage);
			StartCoroutine(destoryObject());
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			StartCoroutine(destoryObject());
		}
	}
}
