using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemGoldMedalData : ItemData{

	public ItemGoldMedalData(int count) : base(5, count)
	{

	}

	override public string Description()
	{
		return "<color=white>"
			 	+ RefItem.name + "\n"
				+ "Count:" + Count
				+ "</color>";
	}
}
