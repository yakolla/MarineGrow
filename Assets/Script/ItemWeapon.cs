﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeapon : Item{

	string 	m_prefWeapon;
	string 	m_weaponName;

	public ItemWeapon(string prefWeapon) : base(Item.Type.Weapon, "Sprites/swordoftruth")
	{
		m_prefWeapon = prefWeapon;
		m_weaponName = Resources.Load<GameObject>(prefWeapon).name;
		LevelUpReqDescs.Add(new LevelUpReqDesc(Item.Type.WeaponFragment, 1));
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

}
