using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	[SerializeField]
	ItemData			m_item;

	Bezier			m_bezier;
	Parabola		m_parabola;
	GameObject		m_target = null;

	void Start () {
		m_parabola = new Parabola(gameObject, Random.Range(-2, 2), Random.Range(5, 7), Random.Range(80, 90), 3);

	}

	void SetTarget(GameObject target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target, transform.position, target.transform.position, 0.07f);
	}

	public void Pickup(Creature obj)
	{
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
