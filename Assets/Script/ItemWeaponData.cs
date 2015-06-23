using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemWeaponData : ItemData{

	string 	m_prefWeapon;
	string 	m_weaponName;

	public ItemWeaponData(int refItemId) : base(refItemId, 1)
	{
		m_prefWeapon = "Pref/Weapon/" + RefItem.codeName;
		m_weaponName = RefItem.codeName;
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


	override public void Equip(Creature obj)
	{
		obj.EquipWeapon(this, null);

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
