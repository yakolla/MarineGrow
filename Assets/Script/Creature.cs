using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour {

	public enum Type
	{
		Champ,
		Mob,
		Npc,
	}

	public enum CrowdControlType
	{
		Nothing = 0x0,
		Airborne = 0x1,
		Stun = 0x2,
		LeapStrike = 0x4,
	}

	int	m_crowdControl = (int)CrowdControlType.Nothing;
	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected WeaponHolder	m_weaponHolder;
	protected Material		m_material;

	public GameObject		m_targeting;

	[SerializeField]
	protected GameObject	m_prefDeathEffect;

	[SerializeField]
	protected Type			m_creatureType;

	GameObject				m_prefDamageGUI;
	GameObject				m_prefPickupItemGUI;
	GameObject				m_prefDamageSprite;

	public CreatureProperty	m_creatureProperty;
	int						m_ingTakenDamageEffect = 0;

	GameObject				m_aimpoint;

	Animator				m_animator;
	
	Spawn					m_spawn;
	RefItemSpawn[]			m_dropItems;
	struct DamageEffect
	{
		public float endTime;
		public GameObject effect;
	}
	DamageEffect[]	m_damageEffects = new DamageEffect[(int)DamageDesc.Type.Count];
	DamageEffect[]	m_pickupItemEffects = new DamageEffect[(int)ItemData.Type.Count];

	DamageEffect[]	m_buffEffects = new DamageEffect[(int)DamageDesc.BuffType.Count];

	float		m_backwardSpeed = 0f;

	Texture damagedTexture;
	Texture normalTexture;

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_aimpoint = transform.Find("Aimpoint").gameObject;

		m_animator = transform.Find("Body").GetComponent<Animator>();

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");
		m_prefPickupItemGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");
		m_prefDamageSprite = Resources.Load<GameObject>("Pref/DamageNumberSprite");
		damagedTexture = Resources.Load<Texture>("ani/damage monster");
		normalTexture = Resources.Load<Texture>("ani/monster");
	}

	public void EquipWeapon(ItemWeaponData weaponData)
	{
		
		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();

		GameObject obj = Instantiate (weaponData.PrefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		Weapon weapon = obj.GetComponent<Weapon>();
		
		obj.transform.parent = m_weaponHolder.transform;
		obj.transform.localPosition = weaponData.PrefWeapon.transform.localPosition;
		obj.transform.localRotation = weaponData.PrefWeapon.transform.localRotation;
		obj.transform.localScale = weaponData.PrefWeapon.transform.localScale;

		weapon.Init(weaponData);
		weapon.m_callbackCreateBullet = delegate() {
			if (m_animator != null)
			{
				m_animator.SetTrigger("Attack");
			}

		};

		m_weaponHolder.EquipWeapon(weapon);

	}

	public Spawn Spawn	{
		set {m_spawn = value;}
		get {return m_spawn;}
	}

	public Vector2 RotateToTarget(Vector3 pos)
	{
		Vector3 gunPoint = m_weaponHolder.transform.position;
		gunPoint.x = transform.position.x;
		gunPoint.z = transform.position.z;
		float targetHorAngle = Mathf.Atan2(pos.z-gunPoint.z, pos.x-gunPoint.x) * Mathf.Rad2Deg;
		Vector3 euler = transform.eulerAngles;
		euler.y = -targetHorAngle;
		transform.eulerAngles = euler;

		return new Vector2(targetHorAngle, 0f);
	}

	public Vector2 RotateToTarget(float angle)
	{	
		Vector3 euler = transform.eulerAngles;
		euler.y = -angle;
		transform.eulerAngles = euler;
		
		return new Vector2(angle, 0f);
	}

	static public bool IsEnemy(Creature a, Creature b)
	{
		if (b.CreatureType == Type.Npc)
			return false;
		if (a.CreatureType == Type.Npc)
			return false;

		return a.CreatureType != b.CreatureType;
	}

	public void EnableNavmesh(bool enable)
	{
		m_navAgent.updatePosition = enable;
		m_navAgent.updateRotation = enable;
	}


	protected void Update()
	{
		UpdateDamageEffect();
		UpdatePickupItemEffect();
		m_navAgent.speed = m_creatureProperty.MoveSpeed;

		if (m_backwardSpeed > 0)
		{
			m_backwardSpeed -= 1f;
			EnableNavmesh(false);
		}
		else
		{
			EnableNavmesh(true);
			rigidbody.velocity = Vector3.zero;
		}


	}

	virtual public string[] GetAutoTargetTags()
	{
		return null;
	}

	public RefItemSpawn[] RefDropItems
	{
		set {m_dropItems = value;}
		get {return m_dropItems;}
	}

	public WeaponHolder WeaponHolder
	{
		get {return m_weaponHolder;}
	}

	protected bool inAttackRange(GameObject targeting, float overrideRange)
	{
		float dist = Vector3.Distance(transform.position, targeting.transform.position);

		if (overrideRange == 0f)
		{
			if (dist <= m_weaponHolder.AttackRange())
			{
				return true;
			}
		}
		else
		{
			if (dist <= overrideRange)
			{
				return true;
			}
		}


		return false;
	}

	public void SetFollowingCamera(GameObject next)
	{
		FollowingCamera followingCamera = Camera.main.GetComponentInChildren<FollowingCamera>();
		followingCamera.SetTarget(gameObject, next);
	}

	public GameObject SearchTarget(string[] targetTags, Creature[] skipTargets, float range)
	{
		foreach(string tag in targetTags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			foreach(GameObject target in targets)
			{
				bool isSkip = false;
				if (skipTargets != null)
				{
					foreach(Creature skip in skipTargets)
					{
						if (skip == null)
							break;

						if (skip.gameObject == target)
						{
							isSkip = true;
							break;
						}
					}
				}

				if (isSkip == true)
				{
					continue;
				}

				if (true == inAttackRange(target, range))
				{
					return target;
				}
			}
		}

		return null;
	}

	protected bool HasCrowdControl()
	{
		return m_crowdControl != (int)CrowdControlType.Nothing;
	}

	virtual public bool AutoAttack() {


		if (HasCrowdControl() == false)
		{
			if (m_targeting != null)
			{
				if (false == inAttackRange(m_targeting, 0f))
				{
					m_targeting = null;
				}
			}

			if (m_targeting == null)
			{
				m_targeting = SearchTarget(GetAutoTargetTags(), null, 0f);
			}

			if (m_targeting != null)
			{
				m_weaponHolder.StartFiring(RotateToTarget(m_targeting.transform.position));
				return true;
			}
		}
		m_targeting = null;
		m_weaponHolder.StopFiring();
		return false;
	}

	protected IEnumerator BodyRedColoredOnTakenDamage()
	{
		Renderer[] renders = GetComponentsInChildren<Renderer>();
		if (renders != null)
		{

			Color color = new Color(0f,1f,0f,0f);
			int len = renders.Length;

			for(int i = 0; i < len; ++i)
			{
				if (renders[i] && renders[i].material && renders[i].material.mainTexture)
				{
					if (renders[i].material.mainTexture.name.CompareTo("monster") == 0)
					{
						renders[i].material.mainTexture = damagedTexture;
					}

				}
			}
			
			yield return new WaitForSeconds(0.3f);
			
			for(int i = 0; i < len; ++i)
			{
				if (renders[i] && renders[i].material && renders[i].material.mainTexture)
				{
					if (renders[i].material.mainTexture.name.CompareTo("damage monster") == 0)
					{
						renders[i].material.mainTexture = normalTexture;
					}
					
				}
			}
		}
		--m_ingTakenDamageEffect;
	}



	void UpdateDamageEffect()
	{
		for(int i = 0; i < (int)DamageDesc.Type.Count; ++i)
		{
			if (m_damageEffects[i].effect != null)
			{
				if (m_damageEffects[i].effect.particleSystem.IsAlive() == false)
				{
					DestroyObject(m_damageEffects[i].effect);
					m_damageEffects[i].effect = null;
				}
			}

		}

	}

	void UpdatePickupItemEffect()
	{
		for(int i = 0; i < (int)ItemData.Type.Count; ++i)
		{
			if (m_pickupItemEffects[i].effect != null)
			{
				if (m_pickupItemEffects[i].effect.particleSystem.IsAlive() == false)
				{
					DestroyObject(m_pickupItemEffects[i].effect);
					m_pickupItemEffects[i].effect = null;
				}
			}
			
		}
		
	}

	public void CrowdControl(CrowdControlType type, bool enable)
	{
		if (enable == true)
		{
			m_crowdControl |= (int)type;
		}
		else
		{
			m_crowdControl &= ~(int)type;
		}
	}

	IEnumerator EffectAirborne()
	{		
		CrowdControl(CrowdControlType.Airborne, true);
		Parabola parabola = new Parabola(gameObject, 0, 7f, 0f, 1.5f, 1);
		while(parabola.Update())
		{

			yield return null;
		}

		m_buffEffects[(int)DamageDesc.BuffType.Airborne].effect = null;
		CrowdControl(CrowdControlType.Airborne, false);
	}

	IEnumerator EffectStun()
	{		
		CrowdControl(CrowdControlType.Stun, true);
		yield return new WaitForSeconds(2f);
		
		m_buffEffects[(int)DamageDesc.BuffType.Stun].effect = null;
		CrowdControl(CrowdControlType.Stun, false);
	}

	IEnumerator EffectSlow(float time)
	{		
		m_creatureProperty.BetaMoveSpeed /= 2f;

		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.Slow].effect = null;
		m_creatureProperty.BetaMoveSpeed *= 2f;

	}




	IEnumerator EffectSteamPack(float time)
	{		
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;
		m_creatureProperty.BetaMoveSpeed *= 2f;
		
		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.SteamPack].effect = null;
		m_creatureProperty.AlphaAttackCoolTime += 0.5f;
		m_creatureProperty.BetaMoveSpeed *= 0.5f;
		
	}

	public void ApplyDamageEffect(DamageDesc.Type type, GameObject prefEffect)
	{
		if (m_damageEffects[(int)type].effect == null && prefEffect != null)
		{
			GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			dmgEffect.transform.parent = m_aimpoint.transform;
			dmgEffect.transform.localPosition = Vector3.zero;
			dmgEffect.transform.particleSystem.startSize = gameObject.transform.localScale.x;
			m_damageEffects[(int)type].effect = dmgEffect;
		}
	}

	public void ApplyBuff(DamageDesc.BuffType type, float time)
	{
		switch(type)
		{
		case DamageDesc.BuffType.Airborne:
			StartCoroutine(EffectAirborne());
			break;
		case DamageDesc.BuffType.Stun:
			StartCoroutine(EffectStun());
			break;
		case DamageDesc.BuffType.Slow:
			StartCoroutine(EffectSlow(time));
			break;
		case DamageDesc.BuffType.SteamPack:
			StartCoroutine(EffectSteamPack(time));
			break;
		}
	}

	public void ApplyPickUpItemEffect(ItemData.Type type, GameObject prefEffect, int value)
	{
		if (m_pickupItemEffects[(int)type].effect == null)
		{
			GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			dmgEffect.transform.parent = m_aimpoint.transform;
			dmgEffect.transform.localPosition = Vector3.zero;
			dmgEffect.transform.particleSystem.startSize = gameObject.transform.localScale.x+prefEffect.transform.localScale.x;
			m_pickupItemEffects[(int)type].effect = dmgEffect;
		}


		string strDamage = value.ToString();			

		switch(type)
		{
		case ItemData.Type.Gold:
			{
			GameObject gui = (GameObject)Instantiate(m_prefDamageSprite, transform.position, m_prefDamageSprite.transform.localRotation);
				DamageNumberSprite sprite = gui.GetComponent<DamageNumberSprite>();
			sprite.Init(gameObject, strDamage, Color.yellow, DamageNumberSprite.MovementType.Up);
			}
			break;
		case ItemData.Type.HealPosion:
			{
			GameObject gui = (GameObject)Instantiate(m_prefDamageSprite, transform.position, m_prefDamageSprite.transform.localRotation);
				DamageNumberSprite sprite = gui.GetComponent<DamageNumberSprite>();
			sprite.Init(gameObject, strDamage, Color.green, DamageNumberSprite.MovementType.Up);
			}
			break;
		}
	}
	
	virtual public void TakeDamage(Creature offender, DamageDesc damageDesc)
	{
		float criticalDamage = 0f;
		if (offender != null)
		{
			if (Random.Range(0, 1f) < offender.m_creatureProperty.CriticalRatio)
			{
				criticalDamage = offender.m_creatureProperty.CriticalDamage;
			}
		}

		int dmg = (int)((damageDesc.Damage+damageDesc.Damage*criticalDamage)-m_creatureProperty.PhysicalDefencePoint);
		dmg = Mathf.Max(0, Mathf.FloorToInt(dmg));
		if (dmg == 0)
		{
			dmg = Random.Range(0, 2);
		}
		
		if (m_ingTakenDamageEffect < Const.ShowMaxDamageNumber)
		{
			++m_ingTakenDamageEffect;

			string strDamage = dmg.ToString();
			if (dmg == 0)
			{
				strDamage = "Block";
			}
			else
			{
				if (criticalDamage > 0f)
				{
					strDamage = "Critical " + dmg.ToString();
				}

			}
			/*
			GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			gui.GetComponent<DamageNumberGUI>().Init(gameObject, strDamage, Color.red, Vector3.zero);
*/



			GameObject gui = (GameObject)Instantiate(m_prefDamageSprite, transform.position, m_prefDamageSprite.transform.localRotation);
			DamageNumberSprite sprite = gui.GetComponent<DamageNumberSprite>();
			sprite.Init(gameObject, strDamage, Color.white, DamageNumberSprite.MovementType.Parabola);

			StartCoroutine(BodyRedColoredOnTakenDamage());
		}

		ApplyDamageEffect(damageDesc.DamageType, damageDesc.PrefEffect);

		if (true == m_creatureProperty.BackwardOnDamage)
		{
			m_backwardSpeed = 10f / rigidbody.mass;
			rigidbody.AddForce(transform.right*-2f, ForceMode.Impulse);
			rigidbody.AddTorque(transform.forward*2f, ForceMode.Impulse);
			rigidbody.maxAngularVelocity = 2f;
			EnableNavmesh(false);
		}

		ApplyBuff(damageDesc.DamageBuffType, 2f);

		if (offender != null)
		{
			offender.m_creatureProperty.Heal((int)(dmg*offender.m_creatureProperty.LifeSteal));
		}

		if (m_creatureProperty.givePAttackDamage(dmg) == 0f)
		{
			if (offender != null)
			{
				offender.GiveExp(m_creatureProperty.Exp);
			}

			Death();
		}

	}

	virtual public void GiveExp(int exp)
	{
		m_creatureProperty.giveExp((int)(exp+exp*m_creatureProperty.GainExtraExp));
	}

	public Type CreatureType
	{
		get { return m_creatureType; }
		set {
			m_creatureType = value;
			tag = m_creatureType.ToString();
		}
	}

	public void ShakeCamera(float time)
	{
		CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();
		shake.shake = time;
		shake.enabled = true;
	}
	
	virtual public void Death()
	{

		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));	
		effect.transform.localScale = transform.localScale;

		DestroyObject(this.gameObject);

		ShakeCamera(0.1f);
	}

}
