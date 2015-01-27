using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemGemData : ItemData{


	public ItemGemData(int gold) : base(8, gold)
	{

	}

	override public void Pickup(Creature obj){
		Use(obj);
	}

	override public void Use(Creature obj){
		Warehouse.Instance.Gem.Item.Count += Count;
	}

}
