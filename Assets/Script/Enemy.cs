using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	NavMeshAgent	m_navAgent;
	Vector3			m_targetPos;
	GameObject		m_prefDamageGUI;
	public			float m_hp = 10f;
	// Use this for initialization
	void Start () {
		m_navAgent = GetComponent<NavMeshAgent>();
		m_prefDamageGUI = Resources.Load<GameObject>("Pref/DamageGUI");
	}
	
	// Update is called once per frame
	void Update () {
		m_navAgent.SetDestination(m_targetPos);
	}

	public void SetTargetPos(Vector3 pos )
	{
		m_targetPos = pos;
	}

	public void TakeDamage(float dmg)
	{
		m_hp -= dmg;
		m_hp = Mathf.Max(0, m_hp);

		if (m_hp == 0)
		{
			Death();
		}
		else{
			GameObject gui = (GameObject)Instantiate(m_prefDamageGUI, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
			gui.GetComponent<DamageGUI>().Init(gameObject, dmg.ToString());
		}
	}

	void Death()
	{
		this.gameObject.GetComponent<LOSEntity>().OnDisable();
		DestroyObject(this.gameObject);
	}
}
