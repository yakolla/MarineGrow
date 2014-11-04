using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected WeaponHolder	m_weaponHolder;
	protected Material		m_material;
	[SerializeField]
	protected float			m_autoTargetCoolTime = 0.5f;
	float					m_lastAutoTargetTime = 0f;
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

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();

		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
		m_weaponHolder.ChangeWeapon(m_prefWeapon);

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");

		m_creatureProperty.init();
	}

	protected float RotateChampToPos(Vector3 pos)
	{
		float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
		transform.eulerAngles =  new Vector3(0, -targetAngle, 0);

		return targetAngle;
	}

	protected bool AutoAttack() {
		if (m_targeting != null)
		{
<<<<<<< HEAD
			RotateChampToPos(m_targeting.transform.position);
			return true;
=======
			float dist = Vector3.Distance(transform.position, m_targeting.transform.position);
			if (dist < m_weaponHolder.GetWeapon().AttackRange)
			{
				m_weaponHolder.GetWeapon().StartFiring(RotateChampToPos(m_targeting.transform.position), 0);
				return true;
			}
>>>>>>> origin/master
		}

		if (m_lastAutoTargetTime+m_autoTargetCoolTime < Time.time)
		{
			m_lastAutoTargetTime = Time.time;

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
		}

		m_weaponHolder.GetWeapon().StopFiring();
		return false;
	}

	virtual protected IEnumerator TakenDamageEffect()
	{
		m_material.color = new Color(0f,0f,0f,0f);
		yield return new WaitForSeconds(0.1f);
		m_material.color = new Color(1f,1f,1f,0f);
<<<<<<< HEAD
=======
		m_ingTakenDamageEffect = false;
>>>>>>> origin/master
	}
	
	virtual public void TakeDamage(Creature offender, float dmg)
	{
		dmg *= 1-m_creatureProperty.PDefencePoint/100f;
		dmg = Mathf.Max(0, Mathf.FloorToInt(dmg));
		
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

			StartCoroutine(TakenDamageEffect());
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
		GameObject effect = (GameObject)Instantiate(m_prefDeathEffect, transform.position, transform.rotation);
		effect.transform.localScale = transform.localScale;

		this.gameObject.GetComponent<LOSEntity>().OnDisable();
		DestroyObject(this.gameObject);
	}

}
