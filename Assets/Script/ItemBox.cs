using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	public enum Type
	{
		Gold,
		HealPosion,
		Weapon,
		Count
	}

	[SerializeField]
	Texture			m_icon;

	protected Type 	m_itemType = Type.Gold;

	[SerializeField]
	int				m_itemValue = 0;

	Bezier			m_bezier;
	Parabola		m_parabola;
	GameObject		m_target;

	protected void Start () {
		m_parabola = new Parabola(gameObject, Random.Range(-2, 2), Random.Range(5, 7), Random.Range(80, 90));
	}

	protected void SetTarget(GameObject target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target.transform.position, transform.position, target.transform.position);
	}

	virtual public void Pickup(Creature obj)
	{
		Use(obj);
		SetTarget(obj.gameObject);
	}

	virtual public string Description()
	{
		return m_itemType.ToString() + "\n" + m_itemValue.ToString();
	}

	virtual public void Use(Creature obj){}

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

	public Type ItemType
	{
		get { return m_itemType; }
	}

	public Texture ItemIcon
	{
		get { return m_icon; }
	}

	public int ItemValue
	{
		get { return m_itemValue; }
		set {m_itemValue = value;}
	}
}
