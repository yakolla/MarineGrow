using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemHealPosion : Item{

	[SerializeField]
	int m_heal;

	public ItemHealPosion(int heal) : base(Item.Type.HealPosion, "Sprites/swordoftruth")
	{
		m_heal = heal;
	}

	override public void Pickup(Creature obj){
		Use(obj);
	}

	override public void Use(Creature obj){
		obj.m_creatureProperty.Heal(m_heal);
	}

}
