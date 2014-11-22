using UnityEngine;
using System.Collections;

public class ItemHealPosionBox : ItemBox {

	new void Start()
	{
		base.Start();
		m_itemType = Type.HealPosion;
	}

	override public void Use(Creature obj)
	{
		obj.m_creatureProperty.Heal(ItemValue);
	}
}
