using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemHealPosionData : ItemData{

	[SerializeField]
	int m_heal;

	public ItemHealPosionData(int heal) : base(2)
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
