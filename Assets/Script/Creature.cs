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

	public enum BehaviourType
	{
		ALive,
		Death,
	}

	protected	BehaviourType	m_behaviourType = BehaviourType.ALive;

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

	GameObject				m_prefDamageSprite;

	public CreatureProperty	m_creatureProperty;
	int						m_ingTakenDamageEffect = 0;

	protected GameObject	m_aimpoint;

	bool					m_checkOnDeath = false;

	Animator				m_animator;
	
	Spawn					m_spawn;
	RefItemSpawn[]			m_dropItems;
	protected struct DamageEffect
	{
		public float endTime;
		public bool	m_run;
	}
	DamageEffect[]	m_damageEffects = new DamageEffect[(int)DamageDesc.Type.Count];

	protected DamageEffect[]	m_buffEffects = new DamageEffect[(int)DamageDesc.BuffType.Count+1];
	float		m_pushbackSpeedOnDamage = 0f;

	Texture damagedTexture;
	Texture normalTexture;

	protected void Start () {
		m_aimpoint = transform.Find("Body/Aimpoint").gameObject;
		m_animator = transform.Find("Body").GetComponent<Animator>();

		m_prefDamageSprite = Resources.Load<GameObject>("Pref/DamageNumberSprite");

		StartCoroutine(EffectShield());
	}

	virtual public void Init()
	{
		m_navAgent = GetComponent<NavMeshAgent>();
		m_targeting = null;
		m_ingTakenDamageEffect = 0;
		m_pushbackSpeedOnDamage = 0;
		m_behaviourType = BehaviourType.ALive;

		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
		m_weaponHolder.Init();

		damagedTexture = Resources.Load<Texture>("ani/damage monster");
		normalTexture = Resources.Load<Texture>("ani/monster");
		ChangeNormalColor();
	}

	Weapon instanceWeapon(ItemWeaponData weaponData)
	{
		GameObject obj = Instantiate (weaponData.PrefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		Weapon weapon = obj.GetComponent<Weapon>();
		
		obj.transform.parent = m_weaponHolder.transform;
		obj.transform.localPosition = weaponData.PrefWeapon.transform.localPosition;
		obj.transform.localRotation = weaponData.PrefWeapon.transform.localRotation;
		obj.transform.localScale = weaponData.PrefWeapon.transform.localScale;
		
		weapon.Init(this, weaponData);

		return weapon;
	}

	public void EquipWeapon(ItemWeaponData weaponData)
	{		
		Weapon weapon = instanceWeapon(weaponData);

		weapon.m_callbackCreateBullet = delegate() {
			if (m_animator != null)
			{
				m_animator.SetTrigger("Attack");
			}
		};

		m_weaponHolder.EquipWeapon(weapon);

	}

	public void EquipPassiveWeapon(ItemWeaponData weaponData)
	{
		Weapon weapon = instanceWeapon(weaponData);
		
		m_weaponHolder.EquipPassiveWeapon(weapon);
	}

	public Vector3	AimpointLocalPos
	{
		get {return m_aimpoint.transform.localPosition;}
	}

	public bool CheckOnDeath
	{
		set {m_checkOnDeath = value;}
		get {return m_checkOnDeath;}
	}

	public float RotateToTarget(Vector3 pos)
	{
		Vector3 gunPoint = m_weaponHolder.transform.position;
		gunPoint.x = transform.position.x;
		gunPoint.z = transform.position.z;
		float targetHorAngle = Mathf.Atan2(pos.z-gunPoint.z, pos.x-gunPoint.x) * Mathf.Rad2Deg;
		Vector3 euler = transform.eulerAngles;
		euler.y = -targetHorAngle;
		transform.eulerAngles = euler;

		return targetHorAngle;
	}

	public float RotateToTarget(float angle)
	{	
		Vector3 euler = transform.eulerAngles;
		euler.y = -angle;
		transform.eulerAngles = euler;
		
		return angle;
	}

	static public bool IsEnemy(Creature a, Creature b)
	{
		if (b.CreatureType == Type.Npc)
			return false;
		if (a.CreatureType == Type.Npc)
			return false;

		return a.CreatureType != b.CreatureType;
	}

	public void EnableNavmeshUpdatePos(bool enable)
	{
		m_navAgent.updatePosition = enable;
		m_navAgent.updateRotation = enable;
	}

	public void EnableNavmesh(bool enable)
	{
		m_navAgent.enabled = enable;
	}

	protected void Update()
	{

		//if (m_navAgent.speed != m_creatureProperty.MoveSpeed)
		{
			m_navAgent.speed = m_creatureProperty.MoveSpeed;
			m_animator.speed = m_creatureProperty.MoveSpeed;
		}


		if (true == m_creatureProperty.BackwardOnDamage)
		{
			if (m_pushbackSpeedOnDamage > 0)
			{
				m_pushbackSpeedOnDamage -= 1f;
				EnableNavmeshUpdatePos(false);
			}
			else
			{
				EnableNavmeshUpdatePos(true);
				rigidbody.velocity = Vector3.zero;
			}
		}

		m_creatureProperty.Update();
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

	void ChangeNormalColor()
	{
		Renderer[] renders = GetComponentsInChildren<Renderer>();
		if (renders != null)
		{
			int len = renders.Length;
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
		}

		yield return new WaitForSeconds(0.3f);
			
		ChangeNormalColor();
		--m_ingTakenDamageEffect;
	}



	IEnumerator UpdateDamageEffect(GameObject effect)
	{
		while(effect.particleSystem.IsAlive())
		{
			yield return null;
		}
		DestroyObject(effect);
	}

	IEnumerator UpdatePickupItemEffect(GameObject effect)
	{
		while(effect.particleSystem.IsAlive())
		{
			yield return null;
		}
		DestroyObject(effect);
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
		
		DamageText(DamageDesc.BuffType.Airborne.ToString(), Color.white, DamageNumberSprite.MovementType.FloatingUp);
		CrowdControl(CrowdControlType.Airborne, true);
		Parabola parabola = new Parabola(gameObject, 8, 0f, 90*Mathf.Deg2Rad, 1);
		while(parabola.Update())
		{
			yield return null;
		}

		m_buffEffects[(int)DamageDesc.BuffType.Airborne].m_run = false;
		CrowdControl(CrowdControlType.Airborne, false);

	}

	IEnumerator EffectStun()
	{		
		CrowdControl(CrowdControlType.Stun, true);
		yield return new WaitForSeconds(2f);
		
		m_buffEffects[(int)DamageDesc.BuffType.Stun].m_run = false;
		CrowdControl(CrowdControlType.Stun, false);
	}

	IEnumerator EffectSlow(float time)
	{		
		
		DamageText(DamageDesc.BuffType.Slow.ToString(), Color.white, DamageNumberSprite.MovementType.FloatingUp);
		m_creatureProperty.BetaMoveSpeed /= 2f;

		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.Slow].m_run = false;
		m_creatureProperty.BetaMoveSpeed *= 2f;


	}

	IEnumerator EffectSteamPack(float time)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ef level up");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;

		m_creatureProperty.BulletLength += 1f;
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;
		m_creatureProperty.BetaMoveSpeed *= 2f;

		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.LevelUp].m_run = false;
		m_creatureProperty.BulletLength -= 1f;
		m_creatureProperty.AlphaAttackCoolTime += 0.5f;
		m_creatureProperty.BetaMoveSpeed *= 0.5f;

		DestroyObject(effect);
	}

	IEnumerator EffectDash(DamageDesc damageDesc, float time)
	{	
		Vector3 pos = transform.position;
		pos.y = damageDesc.PrefEffect.transform.position.y;
		GameObject dmgEffect = (GameObject)Instantiate(damageDesc.PrefEffect, pos, damageDesc.PrefEffect.transform.rotation);

		float finished = Time.time + time;
		rigidbody.AddForce(damageDesc.Dir*10f, ForceMode.Impulse);
		while(Time.time < finished)
		{
			yield return null;
		}
		
		rigidbody.velocity = Vector3.zero;
		m_buffEffects[(int)DamageDesc.BuffType.Dash].m_run = false;
		DestroyObject(dmgEffect);
	}

	IEnumerator EffectBurning(float time, Creature offender, DamageDesc damageDesc)
	{		
		while(time > 0)
		{
			yield return new WaitForSeconds(0.3f);
			time -= 0.3f;
			damageDesc.PushbackOnDamage = false;
			TakeDamage(offender, damageDesc);
		}
		m_buffEffects[(int)DamageDesc.BuffType.Burning].m_run = false;
	}

	IEnumerator EffectCombo100(float time)
	{
		GameObject pref = Resources.Load<GameObject>("Pref/ef combo skill");
		GameObject effect = (GameObject)Instantiate(pref);
		effect.transform.parent = transform;
		effect.transform.localPosition = pref.transform.position;
		effect.transform.localRotation = pref.transform.rotation;

		m_creatureProperty.BulletLength += 1f;
		m_creatureProperty.AlphaAttackCoolTime -= 0.5f;
		m_creatureProperty.BetaMoveSpeed *= 2f;
		
		yield return new WaitForSeconds(time);
		
		m_buffEffects[(int)DamageDesc.BuffType.Combo100].m_run = false;
		m_creatureProperty.AlphaAttackCoolTime += 0.5f;
		m_creatureProperty.BulletLength -= 1f;
		m_creatureProperty.BetaMoveSpeed *= 0.5f;

		DestroyObject(effect);
	}

	IEnumerator EffectShield()
	{
		GameObject pref = null;
		GameObject effect = null;
		DamageNumberSprite sprite = null;

		while(true)
		{
			bool shield = m_creatureProperty.Shield > 0;
			if (shield == true)
			{
				if (pref == null)
				{
					pref = Resources.Load<GameObject>("Pref/ef shield skill");
					effect = (GameObject)Instantiate(pref);
					effect.transform.parent = transform;
					effect.transform.localPosition = pref.transform.position;
					effect.transform.localRotation = pref.transform.rotation;		
					sprite = DamageText("", Color.white, DamageNumberSprite.MovementType.FloatingUpAlways);

					Vector3 scale = sprite.gameObject.transform.localScale;
					scale *= 0.5f;
					sprite.gameObject.transform.localScale = scale;
				}


				sprite.Text = "Shield " + m_creatureProperty.Shield;
			}

			if (pref != null)
			{
				sprite.gameObject.SetActive(shield);
				effect.gameObject.SetActive(shield);
			}

			yield return null;
		}
	}

	void ApplyDamageEffect(DamageDesc.Type type, GameObject prefEffect)
	{
		if (prefEffect == null)
			return;

		GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		dmgEffect.transform.parent = m_aimpoint.transform;
		dmgEffect.transform.localPosition = Vector3.zero;
		dmgEffect.transform.localScale = m_aimpoint.transform.localScale;
		StartCoroutine(UpdateDamageEffect(dmgEffect));	
	}

	virtual public bool ApplyBuff(Creature offender, DamageDesc.BuffType type, float time, DamageDesc damageDesc)
	{
		if (m_buffEffects[(int)type].m_run == true)
			return false;

		m_buffEffects[(int)type].m_run = true;

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
		case DamageDesc.BuffType.LevelUp:
			StartCoroutine(EffectSteamPack(time));
			break;
		case DamageDesc.BuffType.Burning:
			StartCoroutine(EffectBurning(time, offender, damageDesc));
			break;
		case DamageDesc.BuffType.Combo100:
			StartCoroutine(EffectCombo100(time));
			break;
		case DamageDesc.BuffType.Dash:
			StartCoroutine(EffectDash(damageDesc, time));
			break;
		}

		return true;
	}

	public void ApplyPickUpItemEffect(ItemData.Type type, GameObject prefEffect, int value)
	{
		GameObject dmgEffect = (GameObject)Instantiate(prefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		dmgEffect.transform.parent = m_aimpoint.transform;
		dmgEffect.transform.localPosition = Vector3.zero;
		dmgEffect.transform.particleSystem.startSize = gameObject.transform.localScale.x+prefEffect.transform.localScale.x;
		StartCoroutine(UpdatePickupItemEffect(dmgEffect));
	
		string strDamage = value.ToString();

		switch(type)
		{
		case ItemData.Type.Gold:
			{
				DamageText("Gold " + strDamage, Color.yellow, DamageNumberSprite.MovementType.RisingUp);
			}
			break;
		case ItemData.Type.HealPosion:
			{
				DamageText("Heal " + strDamage, Color.green, DamageNumberSprite.MovementType.RisingUp);
			}
			break;
		case ItemData.Type.XPPotion:
			{
				DamageText("XP " + strDamage, Color.blue, DamageNumberSprite.MovementType.RisingUp);
			}
			break;
		}
	}

	public DamageNumberSprite DamageText(string damage, Color color, DamageNumberSprite.MovementType movementType)
	{
		GameObject gui = (GameObject)GameObjectPool.Instance.Alloc(m_prefDamageSprite, m_aimpoint.transform.position, m_prefDamageSprite.transform.localRotation);
		DamageNumberSprite sprite = gui.GetComponent<DamageNumberSprite>();
		sprite.Init(this, damage, color, movementType);

		return sprite;
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

		if (m_buffEffects[(int)DamageDesc.BuffType.Dash].m_run == true)
		{
			dmg = 0;
		}

		bool shielded = false;
		if (m_creatureProperty.Shield > 0)
		{
			dmg = 0;
			--m_creatureProperty.Shield;
			shielded = true;
		}
		
		if (m_ingTakenDamageEffect < Const.ShowMaxDamageNumber)
		{
			++m_ingTakenDamageEffect;
			Color color = Color.white;
			string strDamage = dmg.ToString();
			if (dmg == 0)
			{
				if (shielded == true)
				{
					strDamage = "Shielded";
				}
				else
				{
					strDamage = "Block";
				}

			}
			else
			{
				if (criticalDamage > 0f)
				{
					strDamage = dmg.ToString();
					color = Color.red;
				}

			}


			DamageText(strDamage, color, DamageNumberSprite.MovementType.Parabola);

			StartCoroutine(BodyRedColoredOnTakenDamage());

			ApplyDamageEffect(damageDesc.DamageType, damageDesc.PrefEffect);
		}

		if (true == m_creatureProperty.BackwardOnDamage && damageDesc.PushbackOnDamage && m_pushbackSpeedOnDamage == 0f)
		{
			m_pushbackSpeedOnDamage = 10f / rigidbody.mass;
			rigidbody.AddForce(transform.right*-2f, ForceMode.Impulse);
			rigidbody.AddTorque(transform.forward*2f, ForceMode.Impulse);
			rigidbody.maxAngularVelocity = 2f;
		
			EnableNavmeshUpdatePos(false);
		}

		ApplyBuff(offender, damageDesc.DamageBuffType, 2f, damageDesc);

		m_creatureProperty.HP-=dmg;
		if (m_creatureProperty.HP == 0)
		{
			if (offender != null)
			{
				int lifeSteal = (int)(offender.m_creatureProperty.LifeSteal);
				if (lifeSteal > 0)
				{
					offender.DamageText(lifeSteal.ToString(), Color.green, DamageNumberSprite.MovementType.RisingUp);
					offender.Heal(lifeSteal);
				}
				Const.GetSpawn().SharePotinsChamps(offender, ItemData.Type.XPPotion, m_creatureProperty.Exp, false);
			}

			Death();
		}

	}

	virtual public void GiveExp(int exp)
	{

	}

	public void Heal(int heal)
	{
		m_creatureProperty.HP += heal;
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
		m_behaviourType = BehaviourType.Death;
		DestroyObject(this.gameObject);

		ShakeCamera(0.1f);
	}

}
