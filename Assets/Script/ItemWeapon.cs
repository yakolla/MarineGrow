using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemWeapon : Item{

	string 	m_prefWeapon;
	string 	m_weaponName;

	public ItemWeapon(string prefWeapon) : base(Item.Type.Weapon, "Sprites/swordoftruth")
	{
		m_prefWeapon = prefWeapon;
		m_weaponName = Resources.Load<GameObject>(prefWeapon).name;
	}

	override public void Use(Creature obj)
	{
		obj.ChangeWeapon(Resources.Load<GameObject>(m_prefWeapon));
	}
	
	override public string Description()
	{
		return m_weaponName;
	}

}
