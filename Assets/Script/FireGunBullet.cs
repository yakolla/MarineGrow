using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	[SerializeField]
	float	m_damageOnTime = 0.3f;

	float			m_lastDamageTime = 0f;
	BoxCollider		m_collider;

	ParticleSystem	m_particleSystem;
	float			m_firingStartTime;

	Vector3			m_oriColliderCenter;
	Vector3			m_oriColliderSize;

	void Awake()
	{
		m_collider = GetComponent<BoxCollider>();
		m_particleSystem = transform.Find("Body/Particle System").GetComponent<ParticleSystem>();
		m_particleSystem.startSpeed *= m_collider.size.x;
		m_particleSystem.startSize *= m_collider.size.x;
		m_damageType = DamageDesc.Type.Fire;

		m_oriColliderCenter = m_collider.center;
		m_oriColliderSize = m_collider.size;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_lastDamageTime+(m_damageOnTime*m_ownerCreature.m_creatureProperty.AttackCoolTime)<Time.time)
		{
			m_collider.enabled = true;

			m_lastDamageTime = Time.time;
		}
		else
		{
			m_collider.enabled = false;
		}

		float t = Mathf.Min(1f, (Time.time - m_firingStartTime)*1.2f);
		m_collider.center = new Vector3(m_oriColliderCenter.x*t, m_collider.center.y, m_collider.center.z);
		m_collider.size = new Vector3(m_oriColliderSize.x*t, m_collider.size.y, m_collider.size.z);

	}

	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		Vector3 scale = transform.localScale;
		base.Init(ownerCreature, gunPoint, damage, targetAngle);

		transform.parent = m_gunPoint.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.x, 0));
		transform.localScale = scale;

	}

	override public void StartFiring()
	{
		base.StartFiring();
		m_firingStartTime = Time.time;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		m_particleSystem.Clear();
	}

	void OnTriggerEnter(Collider other) 
	{

		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}
	}

}
