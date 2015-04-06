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

	[SerializeField]
	GameObject					m_prefGunPointEffect = null;
	ParticleSystem				m_gunPointEffect;

	protected	bool			m_firing = false;

	[SerializeField]
	protected float				m_coolTime = 0.5f;

	protected float				m_lastCreated = 0;
	protected Creature 			m_creature;

	[SerializeField]
	float						m_damageRatio = 1f;

	[SerializeField]
	Vector2						m_oriChargingSpeed = new Vector2(0.2f, 0.7f);

	Vector2						m_chargingSpeed;

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
		m_lastCreated = Time.time;
		m_chargingSpeed += m_oriChargingSpeed;

		if (m_prefGunPointEffect != null)
		{

			GameObject obj = Instantiate (m_prefGunPointEffect, Vector3.zero, transform.rotation) as GameObject;
			
			obj.transform.parent = m_gunPoint.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = m_prefGunPointEffect.transform.localScale;
			obj.transform.localRotation = m_prefGunPointEffect.transform.localRotation;
			m_gunPointEffect = obj.GetComponent<ParticleSystem>();

		}
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

	virtual public void MoreFire()
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
			//m_damageRatio += m_level / 2;
			m_creature.m_creatureProperty.AlphaAttackCoolTime -= 0.1f;
		}

		if (m_level % 3 == 0)
		{
			m_chargingSpeed.x += 0.01f;
		}
		m_chargingSpeed.y += (1-m_oriChargingSpeed.y)/(Const.ItemMaxLevel+1);
	}

	public int Damage
	{
		get {return (int)(m_creature.m_creatureProperty.PhysicalAttackDamage*m_damageRatio);}
	}

	protected void playGunPointEffect()
	{
		if (m_gunPointEffect != null)
		{
			m_gunPointEffect.gameObject.SetActive(true);
			if (m_gunPointEffect.isPaused || m_gunPointEffect.isStopped)
			{
				m_gunPointEffect.Play();
			}
			
		}
	}

	protected void stopGunPointEffect()
	{
		if (m_gunPointEffect != null)
		{
			m_gunPointEffect.gameObject.SetActive(false);
			m_gunPointEffect.Stop();
		}
	}

	virtual public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		GameObject obj = Instantiate (m_prefBullet, startPos, Quaternion.Euler(0, transform.rotation.eulerAngles.y+targetAngle.y, 0)) as GameObject;
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_gunPoint, Damage, targetAngle);
		obj.transform.localScale = m_prefBullet.transform.localScale;

		playGunPointEffect();

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

		if (m_gunPointEffect != null)
		{
			//m_gunPointEffect.gameObject.SetActive(false);
			//m_gunPointEffect.Stop();
		}

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

	public Vector2 ChargingSpeed
	{
		get {return m_chargingSpeed;}
	}
}

