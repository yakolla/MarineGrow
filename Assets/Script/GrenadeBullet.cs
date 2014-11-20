using UnityEngine;
using System.Collections;

public class GrenadeBullet : Bullet {

	bool m_isDestroying = false;
	GameObject		m_shadow;

	GameObject m_prefShadow;
	Parabola	m_parabola;
	// Use this for initialization
	void Start () {
		m_parabola = new Parabola(gameObject, 7f, 45f);

	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		targetAngle.y = 0;
		base.Init(ownerCreature, gunPoint, damage, chargingTime, targetAngle);

		m_prefShadow = Resources.Load<GameObject>("Pref/shadow");
		m_shadow = (GameObject)Instantiate(m_prefShadow, transform.position, m_prefShadow.transform.rotation);

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
	
	IEnumerator destoryObject()
	{
		m_isDestroying = true;
		yield return new WaitForSeconds (0.4f);
		DestroyObject(m_shadow);
		DestroyObject(this.gameObject);

	}

	void bomb()
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag(m_targetTagName.ToString());
		Vector3 pos = transform.position;
		//pos.y = 0;
		foreach(GameObject target in targets)
		{
			float dist = Vector3.Distance(pos, target.transform.position);
			if (dist < 5f)
			{
				Creature creature = target.GetComponent<Creature>();
				creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, PrefDamageEffect));
			}
		}

		StartCoroutine(destoryObject());
	}
	
	void OnTriggerEnter(Collider other) {
		return;
		if (other.tag.CompareTo(m_targetTagName.ToString()) == 0)
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
