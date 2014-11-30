using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item {

	public enum Type
	{
		Gold,
		HealPosion,
		Weapon,
		Count
	}

	public enum Option
	{
		PhysicalDmg,
		DefencePoint,
		MovingSpeed,
		Count
	}

	[System.Serializable]
	public struct OptionDesc
	{
		Option	m_type;
		float	m_value;

		public OptionDesc(Option	type, float	value)
		{
			m_type = type;
			m_value = value;
		}

		public Option Type
		{
			get {return m_type;}
		}

		public float Value
		{
			get {return m_value;}
		}

		public string Descript()
		{
			return m_type.ToString() + ":" + m_value.ToString();
		}
	}

	string			m_icon = null;
	Type 			m_itemType = Type.Gold;
	List<OptionDesc>	m_optionDescs = new List<OptionDesc>();

	public Item(Type type, string icon)
	{
		m_icon = icon;
		m_itemType = type;
	}

	virtual public string Description()
	{
		return m_itemType.ToString() + OptionsDescription();
	}

	protected string OptionsDescription()
	{
		if (OptionDescs.Count == 0)
			return "";

		string desc = "\n";
		foreach(OptionDesc op in OptionDescs)
		{
			desc += op.Descript() + "\n";
		}

		return desc;
	}

	virtual public void Pickup(Creature obj){}
	virtual public void Use(Creature obj){}

	public Type ItemType
	{
		get { return m_itemType; }
	}

	public string ItemIcon
	{
		get { return m_icon; }
	}

	public List<OptionDesc> OptionDescs
	{
		get {return m_optionDescs;}
	}

}
