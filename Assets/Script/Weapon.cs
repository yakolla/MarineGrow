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

	protected float				m_lastCreated = 0;
	protected Creature 			m_creature;

	[SerializeField]
	float						m_damageRatio = 1f;

	public delegate void CallbackOnCreateBullet();
	public CallbackOnCreateBullet	m_callbackCreateBullet = delegate(){};

	[SerializeField]
	protected float		m_attackRange;

	int					m_evolution;
	protected int		m_level;

	RefItem				m_refItem;


	protected void Start()
	{
		m_gunPoint = this.transform.parent.transform.gameObject;


	}

	public void Init(ItemWeaponData weaponData)
	{
		m_creature = this.transform.parent.transform.parent.gameObject.GetComponent<Creature>();

		m_refItem = weaponData.RefItem;

		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = 0;
		desc.delayTime = 0;

		m_firingDescs.Add(desc);

		AttackRange = weaponData.WeaponStat.range;
		CoolTime = weaponData.WeaponStat.coolTime;
	
		for(int i = 0; i < weaponData.WeaponStat.firingCount; ++i)
			MoreFire();

		for(int i = 0; i < weaponData.Evolution; ++i)
			Evolution();

		for(int i = 0; i < weaponData.Level; ++i)
			LevelUp();



	}

	public void MoreFire()
	{
		if (m_refItem.evolutionFiring == null)
			return;

		int count = m_firingDescs.Count;

		float angle = m_refItem.evolutionFiring.angle*((count+1)/2);
		if (count % 2 == 1)
		{
			angle *= -1;
		}
		
		float delay = m_refItem.evolutionFiring.delay*count;
		
		
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = angle;
		desc.delayTime = delay;
		
		m_firingDescs.Add(desc);
	}

	public void Evolution()
	{
		++m_evolution;

	}

	virtual public void LevelUp()
	{
		++m_level;
		if (m_level % 2 == 0)
		{
			MoreFire();
		}
		else
		{
			m_damageRatio += m_level / 2;
			m_creature.m_creatureProperty.AlphaAttackCoolTime -= 0.1f;
		}
	}

	public int Damage
	{
		get {return (int)(m_creature.m_creatureProperty.PhysicalAttackDamage*m_damageRatio);}
	}

	virtual public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		GameObject obj = Instantiate (m_prefBullet, startPos, Quaternion.Euler(0, transform.rotation.eulerAngles.y+targetAngle.y, 0)) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_gunPoint, Damage, targetAngle);
		obj.transform.localScale = m_prefBullet.transform.localScale;


		this.audio.Play();

		m_callbackCreateBullet();
		return bullet;
	}

	protected IEnumerator DelayToStartFiring(Vector2 targetAngle, float delay)
	{
		yield return new WaitForSeconds(delay);

		CreateBullet(targetAngle, m_gunPoint.transform.position);

	}

	protected bool isCoolTime()
	{
		return m_lastCreated + (m_coolTime*m_creature.m_creatureProperty.AttackCoolTime) < Time.time;
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
				targetAngle.y = m_firingDescs[i].angle;
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

