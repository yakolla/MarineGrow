using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	[SerializeField]
	ItemData			m_item;


	[SerializeField]
	float			m_lifeTime = 15f;
	float			m_timeToDeath = 0;

	Bezier			m_bezier;
	Parabola		m_parabola;
	Creature		m_target = null;

	[SerializeField]
	GameObject		m_prefPickupItemEffect;

	void Start () {
		m_parabola = new Parabola(gameObject, Random.Range(0f, 2f), Random.Range(5f, 7f), Random.Range(-3.14f, 3.14f), Random.Range(1.3f, 1.57f), 2);
		m_parabola.GroundY = 0.5f;
		m_timeToDeath = Time.time + m_lifeTime;
	}

	void SetTarget(Creature target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target.gameObject, transform.position, target.transform.position, 0.07f);
	}

	public void Pickup(Creature obj)
	{
		m_item.Pickup(obj);
		SetTarget(obj);
	}

	public void Use(Creature obj){
		m_item.Equip(obj);
	}

	public string Description()
	{
		return m_item.Description();
	}

	void Update()
	{
		if (m_target == null)
		{
			if (m_parabola.Update() == false)
			{
				if (Time.time > m_timeToDeath)
					Death();
			}
		}
		else
		{
			if (m_bezier.Update() == false)
			{

				m_target.ApplyPickUpItemEffect(ItemType, m_prefPickupItemEffect, Item.Count);

				Death();
			}
		}

	}

	public void Death()
	{
		DestroyObject(this.gameObject);
	}

	public ItemData.Type ItemType
	{
		get { return m_item.RefItem.type; }
	}

	public ItemData Item
	{
		get { return m_item; }
		set {
			m_item = value;
			GameObject pref = Resources.Load<GameObject>("Pref/ItemBox/ef " + m_item.RefItem.codeName + " eat");
			if (pref != null)
			{
				m_prefPickupItemEffect = pref;
			}
		}
	}


}
