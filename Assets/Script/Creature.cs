using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creature : MonoBehaviour {

	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected WeaponHolder	m_weaponHolder;
	protected Material		m_material;

	public GameObject		m_targeting;
	[SerializeField]
	protected GameObject	m_prefWeapon;

	[SerializeField]
	protected GameObject	m_prefDeathEffect;

	[SerializeField]
	protected string		m_targetTagName;

	GameObject				m_prefDamageGUI;
	public CreatureProperty	m_creatureProperty;
	bool					m_ingTakenDamageEffect = false;

	GameObject				m_floatingHealthBar;

	struct DamageEffect
	{
		public float endTime;
		public GameObject effect;
	}
	DamageEffect[]	m_damageEffects = new DamageEffect[(int)DamageDesc.Type.Count];

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();

		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
		m_weaponHolder.ChangeWeapon(m_prefWeapon);

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");

		GameObject prefFloatingHealthBar = Resources.Load<GameObject>("Pref/FloatingHealthBarGUI");
		m_floatingHealthBar = (GameObject)Instantiate(prefFloatingHealthBar, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
		m_floatingHealthBar.transform.parent = transform;
		m_floatingHealthBar.transform.localPosition = Vector3.zero;

		m_creatureProperty.init();
	}

	protected Vector2 RotateChampToPos(Vector3 pos)
	{
		float targetHorAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
		transform.eulerAngles =  new Vector3(0, -targetHorAngle, 0);

		Debug.Log((pos - transform.position).normalized);
		float targetVerAngle = ((pos - transform.position).normalized * 90).y;
		return new Vector2(targetHorAngle, targetVerAngle);
	}

	protected void Update()
	{
		UpdateDamageEffect();
	}

	protected bool AutoAttack() {
		if (m_targeting != null)
		{
			float dist = Vector3.Distance(transform.position, m_targeting.transform.position);
			if (dist < m_weaponHolder.GetWeapon().AttackRange)
			{
				m_weaponHolder.GetWeapon().StartFiring(RotateChampToPos(m_targeting.transform.position), 0);
				return true;
			}
		}

		GameObject[] targets = GameObject.FindGameObjectsWithTag(m_targetTagName);
		foreach(GameObject target in targets)
		{
			float dist = Vector3.Distance(transform.position, target.transform.position);
			if (dist < m_weaponHolder.GetWeapon().AttackRange)
			{
				m_targeting = target.gameObject;
				m_weaponHolder.GetWeapon().StartFiring(RotateChampToPos(m_targeting.transform.position), 0);
				return true;
			}
		}


		m_weaponHolder.GetWeapon().StopFiring();
		return false;
	}

	protected IEnumerator BodyRedColoredOnTakenDamage()
	{
		m_material.color = new Color(1f,0f,0f,0f);
		yield return new WaitForSeconds(0.3f);
		m_material.color = new Color(1f,1f,1f,0f);
		m_ingTakenDamageEffect = false;
	}

	void UpdateDamageEffect()
	{
		for(int i = 0; i < (int)DamageDesc.Type.Count; ++i)
		{
			if (m_damageEffects[i].effect != null)
			{
				if (m_damageEffects[i].endTime < Time.time)
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
		dmg *= 1-m_creatureProperty.PDefencePoint/100f;
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

			if (damageDesc.PrefEffect != null)
			{
				if (m_damageEffects[(int)damageDesc.DamageType].effect == null)
				{
					GameObject dmgEffect = (GameObject)Instantiate(damageDesc.PrefEffect, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
					dmgEffect.transform.parent = transform;
					dmgEffect.transform.localPosition = Vector3.zero;
					
					m_damageEffects[(int)damageDesc.DamageType].effect = dmgEffect;

				}

				m_damageEffects[(int)damageDesc.DamageType].endTime = Time.time+0.5f;
			}
		}

		if (m_creatureProperty.givePAttackDamage(dmg) == 0f)
		{
			offender.m_creatureProperty.giveExp(m_creatureProperty.Exp);
			Death();
		}
	}

	public string TargetTagName
	{
		get { return m_targetTagName; }
	}
	
	virtual public void Death()
	{
		GameObject.Find("Background").gameObject.GetComponent<background>().SpawnItemBox(transform.position);

		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;

		this.gameObject.GetComponent<LOSEntity>().OnDisable();
		DestroyObject(this.gameObject);

		CameraShake shake = Camera.main.gameObject.GetComponent<CameraShake>();
		shake.shake = 0.1f;
		shake.enabled = true;
	}

}
