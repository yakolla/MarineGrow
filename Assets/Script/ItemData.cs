using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


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
		DamageMultiply,
		DefencePoint,
		MovingSpeed,
		Weapon,
		Count
	}



	int				m_refItemId;
	SecuredType.XInt	m_count = 0;
	SecuredType.XInt	m_level = 1;
	bool			m_lock = false;

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
				"</color>" +
				OptionsDescription();
	}

	protected string OptionsDescription()
	{
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return "";

		string desc = "\n";
		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			desc += (Level >= op.level ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv" + op.level + ":";
			if (op.option.type == Option.Weapon)
			{
				desc += RefData.Instance.RefItems[(int)op.option.value].codeName + "</color>\n";
			}
			else
			{
				desc += op.option.type.ToString() + ":" + op.option.value + "</color>\n";
			}

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
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return;

		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			if (op.level > Level)
				continue;

			switch(op.option.type)
			{
			case Option.DamageMultiply:
				obj.m_creatureProperty.DamageRatio += op.option.value;
				break;
			case Option.MovingSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed += op.option.value;
				break;
			case Option.DefencePoint:
				obj.m_creatureProperty.AlphaPhysicalDefencePoint += (int)op.option.value;
				break;
			case Option.Weapon:
				obj.EquipPassiveSkillWeapon(new ItemWeaponData((int)op.option.value), null);
				break;
			}
		}
	}

	public void NoApplyOptions(Creature obj)
	{
		if (m_refItem.levelup == null || m_refItem.levelup.optionPerLevel == null)
			return;
		
		foreach(RefPriceCondition.RefOptionPerLevel op in m_refItem.levelup.optionPerLevel)
		{
			if (op.level > Level)
				continue;

			switch(op.option.type)
			{
			case Option.DamageMultiply:
				obj.m_creatureProperty.DamageRatio -= op.option.value;
				break;
			case Option.MovingSpeed:
				obj.m_creatureProperty.AlphaMoveSpeed -= op.option.value;
				break;
			case Option.DefencePoint:
				obj.m_creatureProperty.AlphaPhysicalDefencePoint -= (int)op.option.value;
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
		}
	}

	public int Level
	{
		get {

			if (Lock == true)
				return 0;

			return m_level.Value;	
		}
		set {
			m_level.Value = value;
		}
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

}
