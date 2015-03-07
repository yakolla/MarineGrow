using UnityEngine;
using System.Collections;

public class MineBullet : Bullet {

	bool m_isDestroying = false;


	[SerializeField]
	GameObject		m_prefBombEffect = null;

	[SerializeField]
	float			m_bombRange = 5f;

	float			m_elapsed = 0f;
	float			m_bombTime = 5f;

	// Use this for initialization
	void Start () {


	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);

	
	
		m_elapsed = Time.time+m_bombTime;

	}

	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		if (m_elapsed < Time.time)
		{
			bomb();
		}
	}
	
	IEnumerator destoryObject(GameObject bombEffect)
	{
		yield return new WaitForSeconds (bombEffect.particleSystem.duration);
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
					creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, m_damageBuffType, PrefDamageEffect));
				}
			}
		}

		GameObject bombEffect = (GameObject)Instantiate(m_prefBombEffect, transform.position, m_prefBombEffect.transform.rotation);
		bombEffect.particleSystem.startSize = m_bombRange*2;
		this.audio.Play();
		StartCoroutine(destoryObject(bombEffect));
	}
}
