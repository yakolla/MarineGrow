using UnityEngine;

public class Cheat {

	public static bool EnableAbilityRollButton
	{
		get{return Warehouse.Instance.CheatLevel >= 1;}
	}

	public static int HowManyAbilityPointOnStart
	{
		get{
			if (Warehouse.Instance.CheatLevel >= 7)
				return 6;
			else if (Warehouse.Instance.CheatLevel >= 6)
				return 3;
			else if (Warehouse.Instance.CheatLevel >= 5)
				return 1;

			return 0;
		}
	}

	public static int HowManyAccessorySlot
	{
		get{
			if (Warehouse.Instance.CheatLevel >= 3)
				return Const.AccessoriesSlots;

			return Const.HalfAccessoriesSlots;
		}
	}

	public static int HowManyAbilityPointRatioOnLevelUp
	{
		get{
			if (Warehouse.Instance.CheatLevel >= 9)
				return 2;
			
			return 1;
		}
	}
}
