using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeaponData : ItemData{

	string 	m_prefWeapon;
	string 	m_weaponName;

	public ItemWeaponData(int refItemId) : base(refItemId)
	{
		m_prefWeapon = "Pref/" + RefItem.codeName;
		m_weaponName = RefItem.codeName;
		LevelUpReqDescs.Add(new LevelUpReqDesc(ItemData.Type.WeaponFragment, 1));
	}

	public string WeaponName
	{
		get{return m_weaponName;}
	}

	override public void Use(Creature obj)
	{
		obj.ChangeWeapon(Resources.Load<GameObject>(m_prefWeapon));
		ApplyOptions(obj);
	}

	override public void NoUse(Creature obj)
	{
		NoApplyOptions(obj);
	}
	
	override public string Description()
	{
		return m_weaponName + "\n" +  base.Description();
	}

	override public bool Compare(ItemData item)
	{
		if (item.RefItem.type != ItemData.Type.Weapon)
			return base.Compare(item);

		ItemWeaponData weapon = (ItemWeaponData)item;
		return WeaponName.CompareTo(weapon.WeaponName) == 0;
	}

}
