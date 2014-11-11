using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	protected GameObject					m_aimpoint;

	[SerializeField]
	GameObject		m_prefBullet;

	protected	bool			m_firing = false;
	[SerializeField]
	protected float				m_coolTime = 0.5f;

	float						m_lastCreated = 0;
	protected float				m_targetAngle = 0;
	Creature 					m_creature;
	float						m_chargingTime;
	[SerializeField]
	protected float		m_attackRange;

	protected void Start()
	{
		m_aimpoint = this.transform.Find("Aimpoint").gameObject;
		m_creature = this.transform.parent.transform.parent.gameObject.GetComponent<Creature>();

	}

	virtual public GameObject CreateBullet(float chargingTime)
	{
		Vector3 pos = m_aimpoint.transform.position;
		GameObject obj = Instantiate (m_prefBullet, pos, transform.rotation) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_aimpoint, m_creature.m_creatureProperty.PAttackDamage, chargingTime);

		m_lastCreated = Time.time;

		return obj;
	}

	public void StartFiring(float targetAngle, float chargingTime)
	{
		m_targetAngle = targetAngle;
		if (m_lastCreated + m_coolTime < Time.time )
		{
			CreateBullet(chargingTime);
		}
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
				CreateBullet(0);
			}
		}

	}

}

