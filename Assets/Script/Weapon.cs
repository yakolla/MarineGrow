using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	protected GameObject					m_gunPoint;

	[SerializeField]
	GameObject		m_prefBullet;

	protected	bool			m_firing = false;
	[SerializeField]
	protected float				m_coolTime = 0.5f;

	float						m_lastCreated = 0;
	Creature 					m_creature;
	float						m_chargingTime;
	Vector2						m_targetAngle;

	[SerializeField]
	protected float		m_attackRange;

	protected void Start()
	{
		m_gunPoint = this.transform.Find("GunPoint").gameObject;
		m_creature = this.transform.parent.transform.parent.gameObject.GetComponent<Creature>();

	}

	virtual public GameObject CreateBullet(Vector2 targetAngle, float chargingTime)
	{
		Vector3 pos = m_gunPoint.transform.position;
		GameObject obj = Instantiate (m_prefBullet, pos, transform.rotation) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_gunPoint, m_creature.m_creatureProperty.PAttackDamage, chargingTime, targetAngle);

		m_lastCreated = Time.time;

		return obj;
	}

	public void StartFiring(Vector2 targetAngle, float chargingTime)
	{
		if (m_lastCreated + m_coolTime < Time.time )
		{
			CreateBullet(targetAngle, chargingTime);

		}
		m_targetAngle = targetAngle;
		m_firing = true;
	}

	virtual public void StopFiring()
	{
		m_firing = false;
	}

	public float AttackRange
	{
		get { return m_attackRange; }
	}
	protected void Update()
	{
		if (m_firing == true)
		{
			if (m_lastCreated + m_coolTime < Time.time )
			{
				CreateBullet(m_targetAngle, 0);
			}
		}

	}

}

