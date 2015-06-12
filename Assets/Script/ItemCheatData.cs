using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class ItemCheatData : ItemData{

	public ItemCheatData() : base(51, 1)
	{
		Lock = true;
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

		string disableColor = "<color=red>";
		string enableColor = "<color=white>";

		int level = Lock == true ? 0 : Level;

		desc +=  "\n" + (level >= 1 ? enableColor : disableColor) + "Enable Roll Button In AbilityGUI</color>";
		desc +=  "\n" + (level >= 2 ? enableColor : disableColor) + "On starting, Give 1 Ability point</color>";
		desc +=  "\n" + (level >= 3 ? enableColor : disableColor) + "On starting, Give 3 Ability points</color>";
		desc +=  "\n" + (level >= 4 ? enableColor : disableColor) + "On starting, Give 6 Ability points</color>";
		desc +=  "\n" + (level >= 5 ? enableColor : disableColor) + "Enable Acessory slot 3</color>";
		desc +=  "\n" + (level >= 6 ? enableColor : disableColor) + "Enable Acessory slot 4</color>";
		desc +=  "\n" + (level >= 7 ? enableColor : disableColor) + "Enable Acessory slot 4</color>";
		desc +=  "\n" + (level >= 8 ? enableColor : disableColor) + "Enable Acessory slot 4</color>";
		desc +=  "\n" + (level >= 9 ? enableColor : disableColor) + "On level up, Give Ability Points twice</color>";
			
		return desc;
	}

}