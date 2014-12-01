using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	[System.Serializable]
	public struct FiringDesc
	{
		public float 	angle;
		public float	delayTime;
	}

	protected GameObject		m_gunPoint;

	[SerializeField]
	GameObject					m_prefBullet = null;

	protected	bool			m_firing = false;

	[SerializeField]
	protected float				m_coolTime = 0.5f;

	float						m_lastCreated = 0;
	Creature 					m_creature;
	float						m_chargingTime;
	Vector2						m_targetAngle;

	public delegate void CallbackOnCreateBullet();
	public CallbackOnCreateBullet	m_callbackCreateBullet = delegate(){};

	[SerializeField]
	protected float		m_attackRange;

	protected void Start()
	{
		m_gunPoint = this.transform.parent.transform.gameObject;
		m_creature = this.transform.parent.transform.parent.gameObject.GetComponent<Creature>();

	}

	virtual public GameObject CreateBullet(Vector2 targetAngle, float chargingTime)
	{
		Vector3 pos = m_gunPoint.transform.position;
		GameObject obj = Instantiate (m_prefBullet, pos, Quaternion.Euler(0, targetAngle.x, 0)) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_gunPoint, m_creature.m_creatureProperty.PhysicalAttackDamage, chargingTime, targetAngle);
		obj.transform.localScale = m_prefBullet.transform.localScale;
		m_lastCreated = Time.time;

		m_callbackCreateBullet();
		return obj;
	}

	protected IEnumerator DelayToStartFiring(Vector2 targetAngle, float chargingTime, float delay)
	{
		yield return new WaitForSeconds(delay);
		CreateBullet(targetAngle, chargingTime);
	}

	public void StartFiring(Vector2 targetAngle, float chargingTime, FiringDesc[] firingDescs)
	{		
		if (m_lastCreated + m_coolTime < Time.time )
		{
			float oriAng = targetAngle.x;
			for(int i = 0; i < firingDescs.Length; ++i)
			{
				float ang = firingDescs[i].angle-oriAng;
				targetAngle.x = ang;
				StartCoroutine(DelayToStartFiring(targetAngle, chargingTime, firingDescs[i].delayTime));
			}


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

}

