using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemGoldData : ItemData{


	public ItemGoldData(int gold) : base(1, gold)
	{

	}

	override public void Pickup(Creature obj){
		Use(obj);
	}

	override public void Use(Creature obj){
		Warehouse.Instance.Gold += Count;
	}

}
