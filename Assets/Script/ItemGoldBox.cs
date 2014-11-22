using UnityEngine;
using System.Collections;

public class ItemGoldBox : ItemBox {

	new void Start()
	{
		base.Start();
		m_itemType = Type.Gold;
	}

	override public void Use(Creature obj)
	{
		obj.m_creatureProperty.Gold += ItemValue;
	}
}
