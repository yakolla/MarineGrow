using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemCheatData : ItemData{

	public ItemCheatData() : base(51, 1)
	{
	}

	override public void Pickup(Creature obj)
	{
		base.Pickup(obj);
		Use (obj);
	}

	override public void Use(Creature obj)
	{

	}

	override public void NoUse(Creature obj)
	{
	}
	
	override public string Description()
	{
		string desc = "<color=white>" + RefItem.codeName + "</color>" + "\n" +  base.Description();

		int level = Lock == true ? 0 : Level;

		desc +=  "\n" + (level >= 1 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv1:Enable Roll Button of Ability</color>";
		desc +=  "\n" + (level >= 3 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv3:Unlock extra acessory slots</color>";
		desc +=  "\n" + (level >= 5 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv5:On starting, Give 1 Ability point</color>";
		desc +=  "\n" + (level >= 6 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv6:On starting, Give 3 Ability points</color>";
		desc +=  "\n" + (level >= 7 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv7:On starting, Give 6 Ability points</color>";
		desc +=  "\n" + (level >= 9 ? Const.EnabledStringColor : Const.DisabledStringColor) + "Lv9:On level up, Give Ability Points twice</color>";
			
		return desc;
	}

}
