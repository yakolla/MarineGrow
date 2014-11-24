using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	[SerializeField]
	Item			m_item;

	Bezier			m_bezier;
	Parabola		m_parabola;
	GameObject		m_target = null;

	void Start () {
		m_parabola = new Parabola(gameObject, Random.Range(-2, 2), Random.Range(5, 7), Random.Range(80, 90));

	}

	void SetTarget(GameObject target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target.transform.position, transform.position, target.transform.position);
	}

	public void Pickup(Creature obj)
	{
		m_item.Use(obj);
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
			m_parabola.Update();
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

	public Item.Type ItemType
	{
		get { return m_item.ItemType; }
	}

	public Item Item
	{
		get { return m_item; }
		set {m_item = value;}
	}


}
