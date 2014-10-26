using UnityEngine;
using System.Collections;

public class FireGunBullet : Bullet {

	public float	m_damageOnTime = 0.3f;
	float			m_lastDamageTime = 0f;
	MeshCollider		m_collider;
	// Use this for initialization
	void Start () 
	{
		m_collider = GetComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_lastDamageTime+m_damageOnTime<Time.time)
		{
			m_collider.enabled = true;
			m_lastDamageTime = Time.time;
		}
		else
		{
			m_collider.enabled = false;
		}

	}

	override public void Init(GameObject aimpoint, string targetTagName, float damage)
	{
		base.Init(aimpoint, targetTagName, damage);
		this.transform.parent = m_aimpoint.transform;
		GameObject pref = Resources.Load<GameObject>("Pref/FireGunBullet");
		this.transform.localPosition = pref.transform.localPosition;
		this.transform.localScale = pref.transform.localScale;
	}

	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(this.gameObject);
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag.CompareTo("Enemy") == 0)
		{
			other.gameObject.GetComponent<Enemy>().TakeDamage(m_damage);
		}
	}
}
