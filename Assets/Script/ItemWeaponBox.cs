using UnityEngine;
using System.Collections;

public class ItemWeaponBox : ItemBox {

	[SerializeField]
	GameObject	m_prefWeapon;

	new void Start()
	{
		base.Start();
		m_itemType = Type.Weapon;
	}

	override public void Use(Creature obj)
	{
		obj.ChangeWeapon(m_prefWeapon);
	}

	override public string Description()
	{
		return m_prefWeapon.name;
	}
}
