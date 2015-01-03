﻿using UnityEngine;
using System.Collections;

public class MobAIRotation : MobAI {

	float time = 0f;
	WeaponHolder	m_weaponHolder;

	override public void Init(Mob mob)
	{
		base.Init(mob);

		m_weaponHolder = mob.transform.Find("WeaponHolder").gameObject.GetComponent<WeaponHolder>();
	}

	// Update is called once per frame
	int Lerp(int start, int end, float time)
	{
		return (int)(start * (1-time) + end * time);
	}
	
	// Update is called once per frame
	override public void Update () {
		int angle  = Lerp(0, 360, time);
		
		m_weaponHolder.GetWeapon().StartFiring(m_mob.RotateToTarget(angle), 0, m_mob.m_firingDescs);
		
		time += Time.deltaTime * 0.1f;
		time -= (int)time;
	}

}
