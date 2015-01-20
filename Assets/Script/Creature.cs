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
	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected WeaponHolder	m_weaponHolder;
	protected Material		m_material;

	public GameObject		m_targeting;

	[SerializeField]
	protected GameObject	m_prefDeathEffect;

	[SerializeField]
	protected Type			m_creatureType;

	[SerializeField]
	public Weapon.FiringDesc[]	m_firingDescs = null;

	GameObject				m_prefDamageGUI;
	public CreatureProperty	m_creatureProperty;
	bool					m_ingTakenDamageEffect = false;

	GameObject				m_aimpoint;

	Animator				m_animator;
	

	struct DamageEffect
	{
		public float endTime;
		public GameObject effect;
	}
	DamageEffect[]	m_damageEffects = new DamageEffect[(int)DamageDesc.Type.Count];

	bool[]	m_debuff = new bool[(int)DamageDesc.DebuffType.Count];

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_aimpoint = transform.Find("Aimpoint").gameObject;

		m_animator = transform.Find("Body").GetComponent<Animator>();

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");
		
		m_navAgent.speed = m_creatureProperty.MoveSpeed;
	}

	public void ChangeWeapon(ItemWeaponData weaponData)
	{
		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();

		GameObject obj = Instantiate (weaponData.PrefWeapon, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;
		Weapon weapon = obj.GetComponent<Weapon>();
		
		obj.transform.parent = m_weaponHolder.transform;
		obj.transform.localPosition = weaponData.PrefWeapon.transform.localPosition;
		obj.transform.localRotation = weaponData.PrefWeapon.transform.localRotation;
		obj.transform.localScale = weaponData.PrefWeapon.transform.localScale;

		weapon.AttackRange = weaponData.RefItem.weapon.range;
		weapon.CoolTime = weaponData.RefItem.weapon.coolTime;

		m_weaponHolder.ChangeWeapon(weapon);
		m_weaponHolder.GetWeapon().m_callbackCreateBullet = delegate() {
			if (m_animator != null)
			{
				m_animator.SetTrigger("Attack");
			}

		};
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
	}

	virtual public string[] GetAutoTargetTags()
	{
		return null;
	}

	protected bool inAttackRange(GameObject targeting, float overrideRange)
	{
		float dist = Vector3.Distance(transform.position, targeting.transform.position);

		if (overrideRange == 0f)
		{
			if (dist <= m_weaponHolder.GetWeapon().AttackRange)
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

	virtual public bool AutoAttack() {

		bool debuff = false;
		foreach(bool b in m_debuff)
		{
			if (b == true)
			{
				debuff = b;
				break;
			}
		}

		if (debuff == false)
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
				m_weaponHolder.GetWeapon().StartFiring(RotateToTarget(m_targeting.transform.position), m_firingDescs);
				return true;
			}
		}
		m_targeting = null;
		m_weaponHolder.GetWeapon().StopFiring();
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

	IEnumerator EffectAirbone()
	{		
		Parabola parabola = new Parabola(gameObject, 0, 7f, 0f, 1.5f, 1);
		while(parabola.Update())
		{

			yield return null;
		}

		m_debuff[(int)DamageDesc.DebuffType.Airbone] = false;
	}
	
	virtual public void TakeDamage(Creature offender, DamageDesc damageDesc)
	{

		float dmg = damageDesc.Damage;
		dmg *= 1-m_creatureProperty.PhysicalDefencePoint/100f;
		dmg= Mathf.Max(0, Mathf.FloorToInt(dmg));
		
		if (m_ingTakenDamageEffect == false)
		{
			m_ingTakenDamageEffect = true;

			string strDamage = dmg.ToString();
			if (dmg == 0)
			{
				strDamage = "Block";
			}

			GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			gui.GetComponent<DamageNumberGUI>().Init(gameObject, strDamage);

			StartCoroutine(BodyRedColoredOnTakenDamage());
		}

		if (damageDesc.PrefEffect != null)
		{
			if (m_damageEffects[(int)damageDesc.DamageType].effect == null)
			{
				GameObject dmgEffect = (GameObject)Instantiate(damageDesc.PrefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
				dmgEffect.transform.parent = m_aimpoint.transform;
				dmgEffect.transform.localPosition = Vector3.zero;
				dmgEffect.transform.particleSystem.startSize = gameObject.transform.localScale.x;
				m_damageEffects[(int)damageDesc.DamageType].effect = dmgEffect;
			}
			
		}

		if (damageDesc.DamageDeBuffType != DamageDesc.DebuffType.Nothing)
		{
			if (m_debuff[(int)damageDesc.DamageDeBuffType] == false)
			{
				m_debuff[(int)damageDesc.DamageDeBuffType] = true;
				StartCoroutine(EffectAirbone());
			}
		}


		if (m_creatureProperty.givePAttackDamage(dmg) == 0f)
		{
			offender.GiveExp(m_creatureProperty.Exp);
			Death();
		}

	}

	virtual public void GiveExp(int exp)
	{
		m_creatureProperty.giveExp(exp);
	}

	public Type CreatureType
	{
		get { return m_creatureType; }
		set {
			m_creatureType = value;
			tag = m_creatureType.ToString();
		}
	}
	
	virtual public void Death()
	{
		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.GetComponentInChildren<ParticleSystem>().startRotation = Random.Range(0, 360);

		DestroyObject(this.gameObject);

		CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();
		shake.shake = 0.1f;
		shake.enabled = true;
	}

}
