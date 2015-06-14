using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ItemMagicOption
{
	ItemData.Option	m_type;
	float	m_value;
	
	public ItemMagicOption(ItemData.Option	type, float	value)
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
		XPPotion,
		MobEgg,
		Gem,
		Accessory,
		ItemPandora,
		RandomAbility,
		Skill,
		Cheat,
		Count
	}

	
	public enum Option
	{
		PhysicalDmg,
		DefencePoint,
		MovingSpeed,
		Weapon,
		Count
	}



	int				m_refItemId;
	SecuredType.XInt	m_count = 0;
	SecuredType.XInt	m_level = 1;
	bool			m_lock = false;

	List<ItemMagicOption>		m_optionDescs = new List<ItemMagicOption>();

	RefItem				m_refItem;

	[JsonConstructor]
	protected ItemData()
	{
	}

	public ItemData(int refItemId, int count)
	{
		RefItemID = refItemId;
		m_count = count;

	}

	virtual public string Description()
	{
		return "<color=white>" + 
				"Level:" + Level + "\n" + 
				//"Evolution:" + Evolution + "\n" + 
				//m_refItem.type.ToString() + OptionsDescription() + 
				OptionsDescription() + 
				"</color>";
	}

	protected string OptionsDescription()
	{
		if (OptionDescs.Count == 0)
			return "";

		string desc = "\n";
		foreach(ItemMagicOption op in OptionDescs)
		{
			desc += op.Descript() + "\n";
		}

		return desc;
	}

	virtual public void Pickup(Creature obj){Warehouse.Instance.PushItem(this);}
	virtual public void Equip(Creature obj){}
	virtual public void Use(Creature obj){}
	virtual public bool Usable(Creature obj){return true;}
	virtual public void NoUse(Creature obj){}
	virtual public bool Compare(ItemData item)
	{
		return item.RefItem.type == RefItem.type;
	}

	public void ApplyOptions(Creature obj)
	{
		foreach(ItemMagicOption desc in m_optionDescs)
		{
			switch(desc.Type)
			{
			case Option.PhysicalDmg:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage += (int)desc.Value;
				break;
			case Option.MovingSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed += desc.Value;
				break;
			case Option.DefencePoint:
				obj.m_creatureProperty.AlphaPhysicalDefencePoint += (int)desc.Value;
				break;
			case Option.Weapon:
				obj.EquipWeapon(new ItemWeaponData((int)desc.Value), null);
				break;
			}
		}
	}

	public void NoApplyOptions(Creature obj)
	{
		foreach(ItemMagicOption desc in m_optionDescs)
		{
			switch(desc.Type)
			{
			case Option.PhysicalDmg:
				obj.m_creatureProperty.AlphaPhysicalAttackDamage -= (int)desc.Value;
				break;
			case Option.MovingSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed -= desc.Value;
				break;
			case Option.DefencePoint:
				obj.m_creatureProperty.AlphaPhysicalDefencePoint -= (int)desc.Value;
				break;
			}
		}
	}

	public int RefItemID
	{
		get {return m_refItemId;}
		set {
			m_refItemId = value;

			m_refItem = RefData.Instance.RefItems[m_refItemId];
			m_optionDescs.Clear();

			if (m_refItem.options != null)
			{
				foreach(RefItemOption itemOption in m_refItem.options)
				{
					m_optionDescs.Add(new ItemMagicOption(itemOption.type, itemOption.value));
				}
			}

		}
	}

	public int Level
	{
		get {	return m_level.Value;	}
		set {m_level.Value = value;}
	}

	public int Count
	{
		get {return m_count.Value;}
		set {m_count.Value = value;}
	}

	[JsonIgnore]
	public RefItem RefItem
	{
		get {return m_refItem;}
	}

	public bool Lock
	{
		get {return m_lock;}
		set {m_lock = value;}

	}

	public List<ItemMagicOption> OptionDescs
	{
		get {return m_optionDescs;}
	}

}
