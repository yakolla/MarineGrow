using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

	// Use this for initialization
	protected NavMeshAgent	m_navAgent;

	protected GameObject	m_weaponHolder;
	protected Material		m_material;
	[SerializeField]
	protected float			m_autoTargetCoolTime = 0.5f;
	float					m_lastAutoTargetTime = 0f;
	GameObject				m_targeting;

	[SerializeField]
	protected GameObject	m_prefWeapon;

	[SerializeField]
	protected string		m_targetTagName;

	GameObject		m_prefDamageGUI;
	public			CreatureProperty	m_creatureProperty;
	bool			m_ingTakenDamageEffect = false;

	protected void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();

		m_weaponHolder = this.transform.Find("WeaponHolder").gameObject;
		m_weaponHolder.GetComponent<WeaponHolder>().ChangeWeapon(m_prefWeapon, m_targetTagName);

		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageNumberGUI");

		m_creatureProperty.init();
	}

	protected void RotateChampToPos(Vector3 pos)
	{
		float targetAngle = Mathf.Atan2(pos.z-transform.position.z, pos.x-transform.position.x) * Mathf.Rad2Deg;
		transform.eulerAngles =  new Vector3(0, -targetAngle, 0);
		m_weaponHolder.GetComponent<WeaponHolder>().GetWeapon().StartFiring(targetAngle);
	}

	protected bool AutoAttack() {
		if (m_targeting != null)
		{
			RotateChampToPos(m_targeting.transform.position);
			return true;
		}

		if (m_lastAutoTargetTime+m_autoTargetCoolTime < Time.time)
		{
			m_lastAutoTargetTime = Time.time;

			GameObject[] targets = GameObject.FindGameObjectsWithTag(m_targetTagName);
			foreach(GameObject target in targets)
			{
				float dist = Vector3.Distance(transform.position, target.transform.position);
				if (dist < 5f)
				{
					m_targeting = target.gameObject;
					RotateChampToPos(m_targeting.transform.position);
					return true;
				}
			}
			
			return false;
		}

		return false;
	}

	virtual protected IEnumerator TakenDamageEffect()
	{
		m_material.color = new Color(0f,0f,0f,0f);
		yield return new WaitForSeconds(0.1f);
		m_material.color = new Color(1f,1f,1f,0f);
	}
	
	virtual public void TakeDamage(float dmg)
	{
		if (m_creatureProperty.takeDamage(dmg) == 0f)
		{
			Death();
		}
		else{
			GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			gui.GetComponent<DamageNumberGUI>().Init(gameObject, dmg.ToString());
			
			if (m_ingTakenDamageEffect == false)
			{
				m_ingTakenDamageEffect = false;
				StartCoroutine(TakenDamageEffect());
			}
		}
	}
	
	virtual public void Death()
	{
		this.gameObject.GetComponent<LOSEntity>().OnDisable();
		DestroyObject(this.gameObject);
	}

}
