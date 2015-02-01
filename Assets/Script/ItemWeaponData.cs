using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponData : ItemData{

	GameObject 	m_prefWeapon;
	string 	m_weaponName;

	public ItemWeaponData(int refItemId) : base(refItemId, 1)
	{
		m_prefWeapon = Resources.Load<GameObject>("Pref/Weapon/" + RefItem.codeName);
		m_weaponName = RefItem.codeName;
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

	override public void Use(Creature obj)
	{
		obj.ChangeWeapon(this);

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
