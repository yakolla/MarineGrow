using UnityEngine;
using System.Collections;

public class RocketLauncherBullet : Bullet {

	float m_speed = 1f;

	float	m_elpasedTime = 0f;
	// Use this for initialization
	void Start () {
	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage+damage*chargingTime, chargingTime, targetAngle);
		transform.FindChild("Body/Particle System").particleSystem.startSize += chargingTime;
		this.GetComponent<BoxCollider>().size += new Vector3(chargingTime, 0, chargingTime);
	}
	// Update is called once per frame
	void Update () {
		transform.Translate(m_elpasedTime, 0, 0, transform);
		m_elpasedTime += Time.deltaTime*0.3f*m_speed;
	}

	IEnumerator destoryObject()
	{
		m_speed = 0;
		yield return new WaitForSeconds (0.3f);
		DestroyObject(this.gameObject);
	}

	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, PrefDamageEffect));
			StartCoroutine(destoryObject());
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			StartCoroutine(destoryObject());
		}
	}
}
