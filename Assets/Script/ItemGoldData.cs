using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemGoldData : ItemData{

	[SerializeField]
	int m_gold;

	public ItemGoldData(int gold) : base(1)
	{
		m_gold = gold;
	}

	override public void Pickup(Creature obj){
		Use(obj);
	}

	override public void Use(Creature obj){
		Warehouse.Instance.Gold += m_gold;
	}

}
