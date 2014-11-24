using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemGold : Item{

	[SerializeField]
	int m_gold;

	public ItemGold(int gold) : base(Item.Type.Gold, "Sprites/swordoftruth")
	{
		m_gold = gold;
	}

	override public void Use(Creature obj){
		obj.m_creatureProperty.Gold += m_gold;
	}

}
