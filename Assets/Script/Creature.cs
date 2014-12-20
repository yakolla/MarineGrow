using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour {

	public enum Type
	{
		Champ,
		Enemy,
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
	
	RefMob				m_spawnDesc;

	struct DamageEffect
	{
		public float endTime;
		public GameObject effect;
	}
	DamageEffect[]	m_damageEffects = new DamageEffect[(int)DamageDesc.Type.Count];

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_aimpoint = transform.Find("Aimpoint").gameObject;

		m_animator = transform.Find("Body").GetComponent<Animator>();

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");

		m_creatureProperty.init();
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

	protected Vector2 RotateToTarget(Vector3 pos)
	{

		Vector3 gunPoint = m_weaponHolder.transform.position;
		//gunPoint.x = transform.position.x;
		//gunPoint.z = transform.position.z;
		float targetHorAngle = Mathf.Atan2(pos.z-gunPoint.z, pos.x-gunPoint.x) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, -targetHorAngle, 0);

		return new Vector2(targetHorAngle, 0f);
	}

	static public bool IsEnemy(Creature a, Creature b)
	{
		return a.CreatureType != b.CreatureType;
	}
	public RefMob SpawnDesc
	{
		get {return m_spawnDesc;}
	}
	
	public void SetSpawnDesc(RefMob spawnDesc)
	{
		m_spawnDesc = spawnDesc;
	}

	protected void Update()
	{
		UpdateDamageEffect();
	}

	virtual public string[] GetAutoTargetTags()
	{
		return null;
	}

	protected bool inAttackRange(GameObject targeting)
	{
		float dist = Vector3.Distance(transform.position, targeting.transform.position);
		if (dist <= m_weaponHolder.GetWeapon().AttackRange)
		{
			return true;
		}

		return false;
	}

	protected GameObject searchTarget()
	{
		string[] tags = GetAutoTargetTags();
		foreach(string tag in tags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			foreach(GameObject target in targets)
			{
				if (true == inAttackRange(target))
				{
					return target;
				}
			}
		}

		return null;
	}

	virtual protected bool AutoAttack() {
		if (m_targeting != null)
		{
			if (false == inAttackRange(m_targeting))
			{
				m_targeting = null;
			}
		}

		if (m_targeting == null)
		{
			m_targeting = searchTarget();
		}

		if (m_targeting != null)
		{
			m_weaponHolder.GetWeapon().StartFiring(RotateToTarget(m_targeting.transform.position), 0, m_firingDescs);

			return true;
		}

		m_targeting = null;
		m_weaponHolder.GetWeapon().StopFiring();
		return false;
	}

	protected IEnumerator BodyRedColoredOnTakenDamage()
	{
		//m_material.color = new Color(1f,0f,0f,0f);
		yield return new WaitForSeconds(0.3f);
		//m_material.color = new Color(1f,1f,1f,0f);
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
				
				m_damageEffects[(int)damageDesc.DamageType].effect = dmgEffect;
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
	}
	
	virtual public void Death()
	{
		GameObject.Find("Dungeon").gameObject.GetComponent<Dungeon>().SpawnItemBox(m_spawnDesc, transform.position);


		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;

		this.gameObject.GetComponent<LOSEntity>().OnDisable();
		DestroyObject(this.gameObject);

		CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();
		shake.shake = 0.1f;
		shake.enabled = true;
	}

}
