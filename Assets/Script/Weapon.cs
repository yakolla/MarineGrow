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
	public List<FiringDesc>		m_firingDescs = new List<FiringDesc>();

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
	GameObject					m_prefSubWeapon;


	Weapon m_subWeapon;

	public delegate void CallbackOnCreateBullet();
	public CallbackOnCreateBullet	m_callbackCreateBullet = delegate(){};

	[SerializeField]
	protected float		m_attackRange;


	int					m_evolution;
	protected int		m_level;

	RefItem				m_refItem;


	protected void Start()
	{
		if (m_prefGunPointEffect != null)
		{
			GameObject obj = Instantiate (m_prefGunPointEffect, Vector3.zero, transform.rotation) as GameObject;
			
			obj.transform.parent = transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = m_prefGunPointEffect.transform.localScale;
			obj.transform.localRotation = m_prefGunPointEffect.transform.localRotation;
			m_gunPointEffect = obj.GetComponent<ParticleSystem>();

		}
	}


	public void Init(Creature creature, ItemWeaponData weaponData)
	{
		m_creature = creature;
		m_gunPoint = creature.WeaponHolder.gameObject;
		m_refItem = weaponData.RefItem;

		Weapon.FiringDesc desc = DefaultFiringDesc();
		m_firingDescs.Clear();
		m_firingDescs.Add(desc);

		m_lastCreated = Time.time;
		m_firing = false;
		m_level = 0;
		m_evolution = 0;

		AttackRange = weaponData.WeaponStat.range;
		CoolTime = weaponData.WeaponStat.coolTime;
	
		for(int i = 0; i < weaponData.WeaponStat.firingCount; ++i)
			MoreFire();

		for(int i = 0; i < weaponData.Evolution; ++i)
			Evolution();

		for(int i = 0; i < weaponData.Level; ++i)
			LevelUp();
	}

	virtual protected Weapon.FiringDesc DefaultFiringDesc()
	{
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = 0;
		desc.delayTime = 0;

		return desc;
	}

	virtual public void MoreFire()
	{
		if (m_refItem.evolutionFiring == null)
			return;

		if (m_level > Const.ItemMaxLevel+3)
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

		}

	}

	public string WeaponName
	{
		get {return m_refItem.codeName;}
	}

	public int Damage
	{
		get {return GetDamage(m_creature.m_creatureProperty);}
	}

	public int GetDamage(CreatureProperty pro)
	{
		return (int)(pro.PhysicalAttackDamage*m_damageRatio);
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

	virtual public Bullet CreateBullet(Weapon.FiringDesc targetAngle, Vector3 startPos)
	{
		GameObject obj = GameObjectPool.Instance.Alloc(m_prefBullet, startPos, Quaternion.Euler(0, transform.rotation.eulerAngles.y+targetAngle.angle, 0));
		Bullet bullet = obj.GetComponent<Bullet>();
		bullet.Init(m_creature, m_gunPoint.transform.position, Damage, targetAngle, m_subWeapon);
		obj.transform.localScale = m_prefBullet.transform.localScale;

		playGunPointEffect();

		this.audio.Play();

		m_callbackCreateBullet();
		return bullet;
	}

	protected IEnumerator DelayToStartFiring(Weapon.FiringDesc targetAngle, float delay)
	{
		yield return new WaitForSeconds(delay);

		CreateBullet(targetAngle, m_gunPoint.transform.position);

	}

	float coolDownTime()
	{
		const float maxCool = 0.5f;
		float levelRatio = (m_level-1)/(float)Const.ItemMaxLevel;
		float coolPerLevel = (1-levelRatio)*1 + levelRatio*maxCool;
		return m_coolTime*m_creature.m_creatureProperty.AttackCoolTime*coolPerLevel;
	}

	protected bool isCoolTime()
	{
		return m_lastCreated +  coolDownTime() <= Time.time;
	}

	protected float remainCoolTimeRatio()
	{
		float cool = coolDownTime();
		return Mathf.Min(1f, 1f-((m_lastCreated + cool)-Time.time)/cool);
	}

	virtual public void StartFiring(float targetAngle)
	{		
		if ( isCoolTime() == true )
		{
			float oriAng = targetAngle;
			float delay = 0f;
			for(int i = 0; i < m_firingDescs.Count; ++i)
			{
				float ang = m_firingDescs[i].angle-oriAng;
				targetAngle = ang;
				//targetAngle.y = m_firingDescs[i].angle;
				StartCoroutine(DelayToStartFiring(m_firingDescs[i], m_firingDescs[i].delayTime));
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

	public void SetSubWeapon(string prefWeapon, int refId)
	{
		if (m_subWeapon != null)
		{
			GameObject.DestroyObject(m_subWeapon.gameObject);
			m_subWeapon = null;
		}

		GameObject subWeaponObj = (GameObject)Instantiate(Resources.Load(prefWeapon));
		m_subWeapon = subWeaponObj.GetComponent<Weapon>();
		m_subWeapon.Init(m_creature, new ItemWeaponData(refId, null));
	}

	public Weapon GetSubWeapon()
	{
		return m_subWeapon;
	}

	public int Level
	{
		get {return m_level;}
	}
}

