using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	[System.Serializable]
	public struct FiringDesc
	{
		public float 	angle;
		public float	delayTime;
	}

	[SerializeField]
	public List<FiringDesc>			m_firingDescs = new List<FiringDesc>();

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

	int					m_evolution;
	int					m_level;

	RefItem				m_refItem;


	protected void Start()
	{
		m_gunPoint = this.transform.parent.transform.gameObject;
		m_creature = this.transform.parent.transform.parent.gameObject.GetComponent<Creature>();

	}

	public void Init(ItemWeaponData weaponData)
	{
		m_refItem = weaponData.RefItem;

		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = 0;
		desc.delayTime = 0;

		m_firingDescs.Add(desc);
	
		for(int i = 0; i < weaponData.Evolution; ++i)
			Evolution();

		AttackRange = weaponData.WeaponStat.range;
		CoolTime = weaponData.WeaponStat.coolTime;
		m_level = weaponData.Level;

	}

	public void Evolution()
	{
		if (m_refItem.evolutionFiring == null)
			return;

		
		++m_evolution;

		float angle = m_refItem.evolutionFiring.angle*((m_evolution+1)/2);
		if (m_evolution % 2 == 1)
		{
			angle *= -1;
		}
		
		float delay = m_refItem.evolutionFiring.delay*m_evolution;


		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = angle;
		desc.delayTime = delay;

		m_firingDescs.Add(desc);

	}



	virtual public GameObject CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		GameObject obj = Instantiate (m_prefBullet, startPos, Quaternion.Euler(0, targetAngle.x, 0)) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		float damageRatio = m_evolution*9+m_level;
		bullet.Init(m_creature, m_gunPoint, m_creature.m_creatureProperty.PhysicalAttackDamage*damageRatio, targetAngle);
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
		return m_lastCreated + (m_coolTime-m_coolTime*m_creature.m_creatureProperty.AttackCoolTime) < Time.time;
	}

	virtual public void StartFiring(Vector2 targetAngle)
	{		
		if ( isCoolTime() == true )
		{
			float oriAng = targetAngle.x;
			float delay = 0f;
			for(int i = 0; i < m_firingDescs.Count; ++i)
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

