using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemXPPotionData : ItemData{

	public ItemXPPotionData(int count) : base(6, count)
	{

	}

	override public void Pickup(Creature obj){
		Equip(obj);
	}
	
	override public void Equip(Creature obj){
		obj.m_creatureProperty.giveExp(Count);
	}
}
