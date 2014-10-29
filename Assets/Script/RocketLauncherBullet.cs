using UnityEngine;
using System.Collections;

public class RocketLauncherBullet : Bullet {

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
		if (other.tag.CompareTo(m_targetTagName) == 0)
		{
			Creature creature = (Creature)other.gameObject.GetComponent(m_targetTagName);
			creature.TakeDamage(m_ownerCreature, m_ownerCreature.m_creatureProperty.PAttackDamage);
			StartCoroutine(destoryObject());
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			StartCoroutine(destoryObject());
		}
	}
}
