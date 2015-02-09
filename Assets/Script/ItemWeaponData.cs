using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponData : ItemData{

	GameObject 	m_prefWeapon;
	string 	m_weaponName;
	WeaponStat	m_weaponStat;

	public ItemWeaponData(int refItemId, WeaponStat	weaponStat) : base(refItemId, 1)
	{
		m_prefWeapon = Resources.Load<GameObject>("Pref/Weapon/" + RefItem.codeName);
		m_weaponName = RefItem.codeName;
		if (weaponStat == null)
			m_weaponStat = RefItem.weaponStat;
		else
			m_weaponStat = weaponStat;

		Lock = true;
	}

	public GameObject PrefWeapon
	{
		get {return m_prefWeapon;}
	}

	public string WeaponName
	{
		get{return m_weaponName;}
	}

	public WeaponStat WeaponStat
	{
		get{return m_weaponStat;}
	}

	override public void Use(Creature obj)
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
