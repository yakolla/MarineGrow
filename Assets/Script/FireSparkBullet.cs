using UnityEngine;
using System.Collections;

public class FireSparkBullet : Bullet {

	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;	


	ParticleSystem	m_particleSystem;
	BoxCollider		m_boxCollider;

	float			m_bombTime = 0f;
	int				m_triggerFrame = 0;
	float			m_duration = 10f;
	float			m_damageOnTime = 1f;
	float			m_dropTime = 0f;
	// Use this for initialization
	void Start () {
		
		m_boxCollider = GetComponent<BoxCollider>();
		m_boxCollider.enabled = false;
		m_particleSystem = m_prefBombEffect.particleSystem;
		m_particleSystem.enableEmission = false;
		m_damageType = DamageDesc.Type.Fire;

	}
	
	// Update is called once per frame
	void Update () {

		if (m_boxCollider.enabled == false && m_dropTime == 0f)
		{
			m_dropTime = Time.time+1f;
			Vector3 scale = Vector3.one;
			scale.x = m_bombRange;
			transform.localScale = scale;
		}
		
		if (m_boxCollider.enabled == false && m_dropTime < Time.time)
		{
			m_particleSystem.enableEmission = true;
			m_boxCollider.enabled = true;
			m_bombTime = Time.time;
			Vector3 rotation = m_prefBombEffect.transform.eulerAngles;
			rotation.y = transform.eulerAngles.y;
			
			GameObject bombEffect = (GameObject)Instantiate(m_prefBombEffect, transform.position, Quaternion.Euler(rotation));
			Vector3 scale = Vector3.one;
			scale.x = m_bombRange*3f;
			bombEffect.transform.localScale = scale;
			
			
			StartCoroutine(destoryObject(bombEffect));
		}

		
	}
	
	IEnumerator destoryObject(GameObject bombEffect)
	{
		yield return new WaitForSeconds (m_duration);
		DestroyObject(this.gameObject);
		DestroyObject(bombEffect);
	}
	
	
	void OnTriggerStay(Collider other) {
		
		
		if (m_triggerFrame != Time.frameCount)
		{
			if (m_bombTime + m_damageOnTime >= Time.time)
			{
				return;
			}
			
			m_bombTime = Time.time;
			m_triggerFrame = Time.frameCount;
		}
		
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}
		
	}
}
