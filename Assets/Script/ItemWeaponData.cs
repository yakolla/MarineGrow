using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemWeaponData : ItemData{

	string 	m_prefWeapon;
	string 	m_weaponName;
	WeaponStat	m_weaponStat;

	public ItemWeaponData(int refItemId, WeaponStat	weaponStat) : base(refItemId, 1)
	{
		m_prefWeapon = "Pref/Weapon/" + RefItem.codeName;
		m_weaponName = RefItem.codeName;
		if (weaponStat == null)
		{
			m_weaponStat = RefItem.weaponStat;
		}
		else
		{
			m_weaponStat = weaponStat;
			if (m_weaponStat.coolTime == 0)
			{
				m_weaponStat.coolTime = RefItem.weaponStat.coolTime;
			}

			if (m_weaponStat.range == 0)
			{
				m_weaponStat.range = RefItem.weaponStat.range;
			}

			if (m_weaponStat.spPerLevel == 0)
			{
				m_weaponStat.spPerLevel = RefItem.weaponStat.spPerLevel;
			}

			if (m_weaponStat.skillId == 0)
			{
				m_weaponStat.skillId = RefItem.weaponStat.skillId;
			}
		}

		Lock = true;
	}

	[JsonIgnore]
	public GameObject PrefWeapon
	{
		get {return Resources.Load<GameObject>(m_prefWeapon);}
	}

	public string WeaponName
	{
		get{return m_weaponName;}
	}

	public WeaponStat WeaponStat
	{
		get{return m_weaponStat;}
	}

	override public void Equip(Creature obj)
	{
		obj.EquipWeapon(this);

		ApplyOptions(obj);
	}

	override public void NoUse(Creature obj)
	{
		NoApplyOptions(obj);
	}
	
	override public string Description()
	{
		return "<color=white>" + m_weaponName + "</color>" + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return WeaponName.CompareTo(weapon.WeaponName) == 0;
	}

}
