using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item {

	public enum Type
	{
		Gold,
		HealPosion,
		Weapon,
		Count
	}

	string			m_icon = null;

	Type 	m_itemType = Type.Gold;

	public Item(Type type, string icon)
	{
		m_icon = icon;
		m_itemType = type;
	}

	virtual public string Description()
	{
		return m_itemType.ToString();
	}

	virtual public void Use(Creature obj){}

	public Type ItemType
	{
		get { return m_itemType; }
	}

	public string ItemIcon
	{
		get { return m_icon; }
	}

}
