using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parasite : Creature {
	[SerializeField]
	int			m_refWeaponItem = 0;

	[SerializeField]
	RefCreatureBaseProperty	m_creatureBaseProperty;

	new void Start () {

		m_creatureProperty.init(this, m_creatureBaseProperty);

		ChangeWeapon(new ItemWeaponData(m_refWeaponItem));
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
