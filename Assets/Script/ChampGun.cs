using UnityEngine;
using System.Collections;

public class ChampGun : Gun {

	[SerializeField]
	float m_bulletSpeed = 6f;

	override public void LevelUp()
	{
		base.LevelUp();
		if (m_level % 2 == 0)
		{

		}
		else
		{
			m_creature.m_creatureProperty.AlphaAttackCoolTime-=0.01f;
		}
	}
	

}
