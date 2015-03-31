using UnityEngine;
using System.Collections;

public class LeapStrikeBullet : Bullet {

	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;
	
	// Use this for initialization
	void Start () 
	{
		
		bomb();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	
	IEnumerator destoryObject(float t)
	{
		yield return new WaitForSeconds (t);
		
		DestroyObject(gameObject);
	}
	
	void bomb()
	{
		string[] tags = m_ownerCreature.GetAutoTargetTags();
		foreach(string tag in tags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			Vector3 pos = transform.position;
			//pos.y = 0;
			foreach(GameObject target in targets)
			{
				float dist = Vector3.Distance(pos, target.transform.position);
				if (dist < m_bombRange)
				{
					Creature creature = target.GetComponent<Creature>();
					GiveDamage(creature);
				}
			}
		}
		
		GameObject bombEffect = (GameObject)Instantiate(m_prefBombEffect, transform.position, m_prefBombEffect.transform.rotation);
		bombEffect.transform.parent = transform;
		bombEffect.transform.localPosition = Vector3.zero;
		bombEffect.transform.localRotation = m_prefBombEffect.transform.rotation;

		ParticleSystem particleSystem = bombEffect.GetComponent<ParticleSystem>();
		particleSystem.startSize = m_bombRange*2;
		this.GetComponent<AudioSource>().Play();
		
		StartCoroutine(destoryObject(particleSystem.duration));
		
	}
}
