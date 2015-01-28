using UnityEngine;
using System.Collections;

public class SpiderWebBullet : Bullet {

	bool m_isDestroying = false;
	GameObject		m_shadow;
	
	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;
	
	float			m_elapsed = 0f;
	float			m_bombTime = 5f;

	[SerializeField]
	float			m_speed = 7f;
	
	[SerializeField]
	int				m_bouncing = 1;
	
	GameObject m_prefShadow;
	Parabola	m_parabola;
	// Use this for initialization
	void Start () {
		
		
	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);
		
		m_prefShadow = Resources.Load<GameObject>("Pref/shadow");
		
		m_shadow = (GameObject)Instantiate(m_prefShadow, transform.position, m_prefShadow.transform.rotation);
		Vector3 scale = Vector3.one;
		scale.x += m_bombRange/2f;
		scale.y += m_bombRange/2f;
		m_shadow.transform.localScale = scale;
		m_parabola = new Parabola(gameObject, Random.Range(1f, m_speed), 10f, -targetAngle.x * Mathf.Deg2Rad, 45f * Mathf.Deg2Rad, m_bouncing);

		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		if (m_parabola.Update() == false)
		{
			bomb();
		}
		m_shadow.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		m_shadow.transform.localScale = m_prefShadow.transform.localScale * ((m_parabola.MaxHeight-transform.position.y+1.5f)/m_parabola.MaxHeight);
		

	}
	
	IEnumerator destoryObject(GameObject bombEffect)
	{
		DestroyObject(m_shadow);
		
		yield return new WaitForSeconds (bombEffect.particleSystem.duration);
		DestroyObject(this.gameObject);
		DestroyObject(bombEffect);
	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, m_damageBuffType, PrefDamageEffect));
		}
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
				if (dist < m_bombRange)
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
