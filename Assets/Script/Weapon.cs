using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	[System.Serializable]
	public struct FiringDesc
	{
		public float 	angle;
		public float	delayTime;
	}

	[SerializeField]
	public FiringDesc[]			m_firingDescs = null;

	protected GameObject		m_gunPoint;

	[SerializeField]
	GameObject					m_prefBullet = null;

	protected	bool			m_firing = false;

	[SerializeField]
	protected float				m_coolTime = 0.5f;

	float						m_lastCreated = 0;
	protected Creature 			m_creature;
	float						m_chargingTime;

	public delegate void CallbackOnCreateBullet();
	public CallbackOnCreateBullet	m_callbackCreateBullet = delegate(){};

	[SerializeField]
	protected float		m_attackRange;

	protected void Start()
	{
		m_gunPoint = this.transform.parent.transform.gameObject;
		m_creature = this.transform.parent.transform.parent.gameObject.GetComponent<Creature>();

	}

	public void Init(ItemWeaponData weaponData)
	{
		if (weaponData.RefItem.evolutionFiring == null)
		{
			m_firingDescs = new Weapon.FiringDesc[1];
			m_firingDescs[0].angle = 0;
			m_firingDescs[0].delayTime = 0;
		}
		else
		{
			m_firingDescs = new Weapon.FiringDesc[weaponData.Evolution*2+1];
			for(int i = 0; i < m_firingDescs.Length; ++i)
			{
				m_firingDescs[i].angle = weaponData.RefItem.evolutionFiring.angle*((i+1)/2);
				if (i % 2 == 1)
				{
					m_firingDescs[i].angle *= -1;
				}
				
				
				m_firingDescs[i].delayTime = weaponData.RefItem.evolutionFiring.delay*i;
				
			}
		}
		
		AttackRange = weaponData.WeaponStat.range;
		CoolTime = weaponData.WeaponStat.coolTime;

	}

	virtual public GameObject CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		GameObject obj = Instantiate (m_prefBullet, startPos, Quaternion.Euler(0, targetAngle.x, 0)) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_gunPoint, m_creature.m_creatureProperty.PhysicalAttackDamage, targetAngle);
		obj.transform.localScale = m_prefBullet.transform.localScale;


		this.audio.Play();

		m_callbackCreateBullet();
		return obj;
	}

	protected IEnumerator DelayToStartFiring(Vector2 targetAngle, float delay)
	{
		yield return new WaitForSeconds(delay);

		CreateBullet(targetAngle, m_gunPoint.transform.position);

	}

	protected bool isCoolTime()
	{
		return m_lastCreated + m_coolTime < Time.time;
	}

	virtual public void StartFiring(Vector2 targetAngle)
	{		
		if ( isCoolTime() == true )
		{
			float oriAng = targetAngle.x;
			float delay = 0f;
			for(int i = 0; i < m_firingDescs.Length; ++i)
			{
				float ang = m_firingDescs[i].angle-oriAng;
				targetAngle.x = ang;
				StartCoroutine(DelayToStartFiring(targetAngle, m_firingDescs[i].delayTime));
				delay = m_firingDescs[i].delayTime;
			}

			m_lastCreated = Time.time+delay;
		}

		m_firing = true;
	}

	virtual public void StopFiring()
	{
		m_firing = false;
	}

	public float CoolTime
	{
		get { return m_coolTime; }
		set { m_coolTime = value; }
	}

	public float AttackRange
	{
		get { return m_attackRange; }
		set { m_attackRange = value; }
	}

}

