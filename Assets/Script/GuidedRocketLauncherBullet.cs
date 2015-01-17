using UnityEngine;
using System.Collections;

public class GuidedRocketLauncherBullet : Bullet {

	[SerializeField]
	float m_speed = 10f;
	Bezier m_bezier = null;
	// Use this for initialization
	void Start () {
	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage+damage, targetAngle);

		m_bezier = new Bezier(this.gameObject, 
		                      ownerCreature.m_targeting, 
		                      this.transform.position+this.transform.forward*3f, 
		                      ownerCreature.m_targeting.transform.position+ownerCreature.m_targeting.transform.forward*-3f,
		                      m_speed/1000f);
	}
	// Update is called once per frame
	void Update () {
		if (m_bezier == null)
			return;

		if (m_bezier.Update() == false)
		{
			StartCoroutine(destoryObject());
		}

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
			creature.TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Normal, DamageDesc.DebuffType.Nothing, PrefDamageEffect));
			StartCoroutine(destoryObject());
		}
		else if (other.tag.CompareTo("Wall") == 0)
		{
			StartCoroutine(destoryObject());
		}
	}
}
