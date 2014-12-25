using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	[SerializeField]
	ItemData			m_item;

	[SerializeField]
	AudioClip			m_sfxPickupItem;

	[SerializeField]
	float			m_lifeTime = 5f;
	float			m_timeToDeath = 0;

	Bezier			m_bezier;
	Parabola		m_parabola;
	GameObject		m_target = null;

	void Start () {
		m_parabola = new Parabola(gameObject, Random.Range(-0.5f, 0.5f), Random.Range(5, 7), Random.Range(80, 90), 50);
		m_timeToDeath = Time.time + m_lifeTime;
	}

	void SetTarget(GameObject target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target, transform.position, target.transform.position, 0.07f);
	}

	public void Pickup(Creature obj)
	{
		audio.clip = m_sfxPickupItem;
		audio.Play();

		m_item.Pickup(obj);
		SetTarget(obj.gameObject);
	}

	public void Use(Creature obj){
		m_item.Use(obj);
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
				Death();
			}
		}
		else
		{
			if (m_bezier.Update() == false)
			{
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
		set {m_item = value;}
	}


}
