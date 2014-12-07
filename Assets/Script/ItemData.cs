using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class OptionDesc
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	ItemData.Option	m_type;
	float	m_value;
	
	public OptionDesc(ItemData.Option	type, float	value)
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

public struct LevelUpReqDesc
{
	[JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
	ItemData.Type	m_type;
	int		m_value;
	
	public LevelUpReqDesc(ItemData.Type	type, int	value)
	{
		m_type = type;
		m_value = value;
	}
	
	public ItemData.Type ItemType
	{
		get {return m_type;}
	}
	
	public int ItemCount
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
		WeaponFragment,
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
	int				m_count = 1;
	int				m_level = 1;

	List<OptionDesc>	m_optionDescs = new List<OptionDesc>();
	List<LevelUpReqDesc>	m_levelupReqDescs = new List<LevelUpReqDesc>();

	[JsonIgnore]
	RefItem				m_refItem;

	public ItemData(int refItemId)
	{
		m_refItemId = refItemId;
		m_refItem = RefData.Instance.RefItems[m_refItemId];
	}

	virtual public string Description()
	{
		return "Level:" + Level + "\n" + m_refItem.type.ToString() + OptionsDescription();
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

	virtual public void Pickup(Creature obj){Warehouse.Instance.PushItem(this);}
	virtual public void Use(Creature obj){}
	virtual public void NoUse(Creature obj){}
	virtual public bool Compare(ItemData item)
	{
		return item.RefItem.type == RefItem.type;
	}

	public void ApplyOptions(Creature obj)
	{
		foreach(OptionDesc desc in m_optionDescs)
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
		foreach(OptionDesc desc in m_optionDescs)
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

	public int Count
	{
		get {return m_count;}
		set {m_count = value;}
	}

	public RefItem RefItem
	{
		get {return m_refItem;}
	}

	public List<OptionDesc> OptionDescs
	{
		get {return m_optionDescs;}
	}

	public List<LevelUpReqDesc> LevelUpReqDescs
	{
		get {return m_levelupReqDescs;}
	}

}
