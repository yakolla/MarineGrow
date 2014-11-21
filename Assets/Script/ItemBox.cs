using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour {

	public enum Type
	{
		Gold,
		HealPosion,
		Ring,
		Count
	}

	[SerializeField]
	Type 			m_itemType = Type.Gold;

	[SerializeField]
	int				m_itemValue = 0;

	Bezier			m_bezier;
	Parabola		m_parabola;
	GameObject		m_target;

	void Start () {
		m_parabola = new Parabola(gameObject, Random.Range(7, 10), Random.Range(1, 4.3f)*20f);
		transform.localEulerAngles = new Vector3(0f, Random.Range(0, 180f), 0f);
	}

	void SetTarget(GameObject target)
	{
		m_target = target;
		m_bezier = new Bezier(gameObject, target.transform.position, transform.position, target.transform.position);
	}

	public void Eaten(Creature obj)
	{
		switch(ItemType)
		{
		case ItemBox.Type.HealPosion:
			obj.m_creatureProperty.Heal(ItemValue);
			break;
		case ItemBox.Type.Gold:
			obj.m_creatureProperty.Gold += ItemValue;
			
			break;
		}

		SetTarget(obj.gameObject);
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

	public Type ItemType
	{
		get { return m_itemType; }
	}

	public int ItemValue
	{
		get { return m_itemValue; }
		set {m_itemValue = value;}
	}
}
