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

	public CreatureProperty	m_creatureProperty;
	bool					m_ingTakenDamageEffect = false;

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

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_aimpoint = transform.Find("Aimpoint").gameObject;

		m_animator = transform.Find("Body").GetComponent<Animator>();

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");
		m_prefPickupItemGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");


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
		//gunPoint.x = transform.position.x;
		//gunPoint.z = transform.position.z;
		float targetHorAngle = Mathf.Atan2(pos.z-gunPoint.z, pos.x-gunPoint.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);

		return new Vector2(targetHorAngle, 0f);
	}

	public Vector2 RotateToTarget(float angle)
	{		
		transform.eulerAngles = new Vector3(0, -angle, 0);
		
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


	protected void Update()
	{
		UpdateDamageEffect();
		UpdatePickupItemEffect();
		m_navAgent.speed = m_creatureProperty.MoveSpeed;
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
			Texture dtex = Resources.Load<Texture>("ani/damage monster");
			Texture tex = Resources.Load<Texture>("ani/monster");
			Color color = new Color(0f,1f,0f,0f);
			int len = renders.Length;

			for(int i = 0; i < len; ++i)
			{
				if (renders[i] && renders[i].material && renders[i].material.mainTexture)
				{
					if (renders[i].material.mainTexture.name.CompareTo("monster") == 0)
					{
						renders[i].material.mainTexture = dtex;
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
						renders[i].material.mainTexture = tex;
					}
					
				}
			}
		}
		m_ingTakenDamageEffect = false;
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

	IEnumerator EffectAirborne()
	{		
		
		m_crowdControl += (int)CrowdControlType.Airborne;
		Parabola parabola = new Parabola(gameObject, 0, 7f, 0f, 1.5f, 1);
		while(parabola.Update())
		{

			yield return null;
		}

		m_buffEffects[(int)DamageDesc.BuffType.Airborne].effect = null;
		m_crowdControl -= (int)CrowdControlType.Airborne;
	}

	IEnumerator EffectStun()
	{		
		m_crowdControl += (int)CrowdControlType.Stun;
		yield return new WaitForSeconds(2f);
		
		m_buffEffects[(int)DamageDesc.BuffType.Stun].effect = null;
		m_crowdControl -= (int)CrowdControlType.Stun;
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
				GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
				Color color = Color.yellow;
				Vector3 offset = new Vector3(0.2f, 0.5f, 0.2f);
				gui.GetComponent<DamageNumberGUI>().Init(gameObject, strDamage, color, offset);
			}
			break;
		case ItemData.Type.HealPosion:
			{
				GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
				Color color = Color.green;
				Vector3 offset = new Vector3(0.5f, 1f, 0.5f);
				gui.GetComponent<DamageNumberGUI>().Init(gameObject, strDamage, color, offset);
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

		float dmg = (damageDesc.Damage+damageDesc.Damage*criticalDamage)-m_creatureProperty.PhysicalDefencePoint;
		dmg = Mathf.Max(0, Mathf.FloorToInt(dmg));
		if (dmg == 0)
		{
			dmg = Random.Range(0, 2);
		}
		
		if (m_ingTakenDamageEffect == false)
		{
			m_ingTakenDamageEffect = true;

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

			GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			gui.GetComponent<DamageNumberGUI>().Init(gameObject, strDamage, Color.red, Vector3.zero);

			StartCoroutine(BodyRedColoredOnTakenDamage());
		}

		ApplyDamageEffect(damageDesc.DamageType, damageDesc.PrefEffect);
		if (m_buffEffects[(int)DamageDesc.BuffType.Slow].effect == null)
		{
			ApplyBuff(DamageDesc.BuffType.Slow, 0.2f);
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
