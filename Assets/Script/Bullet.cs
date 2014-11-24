using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	protected GameObject	m_gunPoint = null;
	bool					m_firing = false;
	protected	float		m_damage;
	protected 	Vector2		m_targetAngle;
	protected	Creature	m_ownerCreature;

	[SerializeField]
	GameObject 		m_prefDamageEffect = null;

	virtual public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		m_gunPoint = gunPoint;
		m_ownerCreature = ownerCreature;
		m_damage = damage;
		m_targetAngle = targetAngle;


		this.transform.parent = m_gunPoint.transform;
		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle.y));
		this.transform.parent = null;

		StartFiring();
	}

	public void StartFiring()
	{
		m_firing = true;

	}

	virtual public void StopFiring()
	{
		m_firing = false;
	}

	public bool IsFiring()
	{
		return m_firing;
	}

	protected GameObject PrefDamageEffect
	{
		get {return m_prefDamageEffect;}
	}

}
