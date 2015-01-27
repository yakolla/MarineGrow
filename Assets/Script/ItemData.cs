using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ItemOptionDesc
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	ItemData.Option	m_type;
	float	m_value;
	
	public ItemOptionDesc(ItemData.Option	type, float	value)
	{
		m_type = type;
		m_value = value;
	}
	
	public ItemData.Option Type
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


public class ItemData {
	
	public enum Type
	{
		Gold,
		HealPosion,
		Weapon,
		WeaponParts,
		WeaponDNA,
		Follower,
		GoldMedal,
		SilverMedal,
		MobEgg,
		Gem,
		Count
	}

	
	public enum Option
	{
		PhysicalDmg,
		DefencePoint,
		MovingSpeed,
		Count
	}



	int				m_refItemId;
	int				m_count = 0;
	int				m_level = 1;
	int				m_evoultion = 0;
	bool			m_lock = false;

	List<ItemOptionDesc>		m_optionDescs = new List<ItemOptionDesc>();

	[JsonIgnore]
	RefItem				m_refItem;

	public ItemData(int refItemId, int count)
	{
		m_refItemId = refItemId;
		m_count = count;
		m_refItem = RefData.Instance.RefItems[m_refItemId];
	}

	virtual public string Description()
	{
		return "Level:" + Level + "\n" + 
				"Evolution:" + Evolution + "\n" + 
				m_refItem.type.ToString() + OptionsDescription();
	}

	protected string OptionsDescription()
	{
		if (OptionDescs.Count == 0)
			return "";

		string desc = "\n";
		foreach(ItemOptionDesc op in OptionDescs)
		{
			desc += op.Descript() + "\n";
		}

		return desc;
	}

	virtual public void Pickup(Creature obj){Warehouse.Instance.PushItem(this);}
	virtual public void Use(Creature obj){}
	virtual public void NoUse(Creature obj){}
	virtual public bool Compare(ItemData item)
	{
		return item.RefItem.type == RefItem.type;
	}

	public void ApplyOptions(Creature obj)
	{
		foreach(ItemOptionDesc desc in m_optionDescs)
		{
			switch(desc.Type)
			{
			case Option.PhysicalDmg:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage += desc.Value;
				break;
			case Option.MovingSpeed:
				break;
			case Option.DefencePoint:
				obj.m_creatureProperty.AlphaPhysicalDefencePoint += desc.Value;
				break;
			}
		}
	}

	public void NoApplyOptions(Creature obj)
	{
		foreach(ItemOptionDesc desc in m_optionDescs)
		{
			switch(desc.Type)
			{
			case Option.PhysicalDmg:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage -= desc.Value;
				break;
			case Option.MovingSpeed:
				break;
			case Option.DefencePoint:
				obj.m_creatureProperty.AlphaPhysicalDefencePoint -= desc.Value;
				break;
			}
		}
	}

	public int Level
	{
		get {return m_level;}
		set {m_level = value;}
	}

	public int Evolution
	{
		get {return m_evoultion;}
		set {m_evoultion = value;}
	}

	public int Count
	{
		get {return m_count;}
		set {m_count = value;}
	}

	public RefItem RefItem
	{
		get {return m_refItem;}
	}

	public bool Lock
	{
		get {return m_lock;}
		set {m_lock = value;}

	}

	public List<ItemOptionDesc> OptionDescs
	{
		get {return m_optionDescs;}
	}

}
