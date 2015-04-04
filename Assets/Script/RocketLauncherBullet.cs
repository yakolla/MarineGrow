using UnityEngine;
using System.Collections;

public class RocketLauncherBullet : Bullet {

	[SerializeField]
	protected float m_speed = 1f;
	protected float	m_accel = 0f;

	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;

	bool m_isDestroying = false;

	// Use this for initialization
	void Start () {
	}


	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		transform.Translate(m_accel, 0, 0, transform);
		m_accel += Time.deltaTime*0.1f*m_speed;


	}

	void OnTriggerEnter(Collider other) {
		if (m_isDestroying == true)
			return;

		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			bomb();
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			DestroyObject(this.gameObject);
		}
	}

	IEnumerator destoryObject(GameObject bombEffect)
	{
		yield return new WaitForSeconds (bombEffect.GetComponent<ParticleSystem>().duration);
		DestroyObject(this.gameObject);
		DestroyObject(bombEffect);
	}

	void bomb()
	{
		m_isDestroying = true;
		
		string[] tags = m_ownerCreature.GetAutoTargetTags();
		foreach(string tag in tags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			Vector3 pos = transform.position;
			//pos.y = 0;
			foreach(GameObject target in targets)
			{
				float dist = Vector3.Distance(pos, target.transform.position);
				if (dist < m_bombRange/2)
				{
					Creature creature = target.GetComponent<Creature>();
					GiveDamage(creature);
				}
			}
		}
		
		GameObject bombEffect = (GameObject)Instantiate(m_prefBombEffect, transform.position, m_prefBombEffect.transform.rotation);
		this.GetComponent<AudioSource>().Play();
		StartCoroutine(destoryObject(bombEffect));

		//gameObject.SetActive(false);
	}
}
