using UnityEngine;
using System.Collections;

public class FloatingWeaponChargingGuageGUI : FloatingGuageBarGUI {

	override protected float guageRemainRatio()
	{
		return m_creature.WeaponHolder.ChargingGuage;
	}
}

