using UnityEngine;
using System.Collections;

public class GrenadeBullet : Bullet {

	float m_speed = 3f;
	float m_startTime;
	bool m_pendingDestroy = false;
	// Use this for initialization
	void Start () {
		m_startTime = Time.time;

	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, chargingTime, targetAngle);
		m_speed += chargingTime;
	}

	// Update is called once per frame
	void Update () {
		if (m_pendingDestroy == true)
			return;

		float elapse = Time.time - m_startTime;
		float y = -2.98f*(elapse*elapse) + 1.5f*elapse;
		transform.Translate(m_speed*Time.deltaTime, y, 0, transform);
	}
	
	IEnumerator destoryObject()
	{
		m_pendingDestroy = true;
		yield return new WaitForSeconds (0.4f);
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
				creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, null));
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
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
