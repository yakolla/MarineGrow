using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parasite : Creature {
	[SerializeField]
	GameObject			m_prefWeapon = null;

	new void Start () {

		m_creatureProperty.init();

		ChangeWeapon(m_prefWeapon);
	}

	// Update is called once per frame
	new void Update () {
		base.Update();
		
		AutoAttack();

	}

	override public string[] GetAutoTargetTags()
	{
		return new string[]{Creature.Type.Champ.ToString()};
	}
}
