using UnityEngine;
using System.Collections;

public class GrenadeBullet : Bullet {

	float m_speed = 7f;
	float m_startTime;
	bool m_isDestroying = false;
	GameObject		m_shadow;
	[SerializeField]
	GameObject 		m_prefDamageEffect;

	float			m_ang = 45f;
	Vector2			m_vel;
	float			m_height;
	float			m_gravity = 10f;
	GameObject m_prefShadow;
	// Use this for initialization
	void Start () {
		m_startTime = Time.time;
		m_vel.x = m_speed * Mathf.Cos(m_ang);
		m_vel.y = m_speed * Mathf.Sin(m_ang);
		m_height = (m_vel.y*m_vel.y)/(2*m_gravity);
	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		targetAngle.y = 0;
		base.Init(ownerCreature, gunPoint, damage, chargingTime, targetAngle);
		m_speed += chargingTime;

		m_prefShadow = Resources.Load<GameObject>("Pref/shadow");
		m_shadow = (GameObject)Instantiate(m_prefShadow, transform.position, m_prefShadow.transform.rotation);

	}

	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		float elapse = Time.time - m_startTime;
		float y = m_vel.y*elapse -0.5f*m_gravity*(elapse*elapse);
		transform.position = new Vector3(transform.position.x, m_gunPoint.transform.position.y +y, transform.position.z);
		transform.Translate(m_vel.x*Time.deltaTime, 0, 0, transform);
		m_shadow.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		m_shadow.transform.localScale = m_prefShadow.transform.localScale * ((m_height-y+1.5f)/m_height);

		if (transform.position.y < 0f)
		{
			bomb();
		}
	}
	
	IEnumerator destoryObject()
	{
		m_isDestroying = true;
		yield return new WaitForSeconds (0.4f);
		DestroyObject(m_shadow);
		DestroyObject(this.gameObject);

	}

	void bomb()
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag(m_targetTagName);
		Vector3 pos = transform.position;
		//pos.y = 0;
		foreach(GameObject target in targets)
		{
			float dist = Vector3.Distance(pos, target.transform.position);
			if (dist < 5f)
			{
				Creature creature = target.GetComponent<Creature>();
				creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, m_prefDamageEffect));
			}
		}

		StartCoroutine(destoryObject());
	}
	
	void OnTriggerEnter(Collider other) {
		return;
		if (other.tag.CompareTo(m_targetTagName) == 0)
		{
			bomb();


			StartCoroutine(destoryObject());
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			bomb();

			StartCoroutine(destoryObject());
		}
	}
}
