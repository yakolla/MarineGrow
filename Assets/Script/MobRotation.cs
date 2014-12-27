using UnityEngine;
using System.Collections;

public class MobRotation : Mob {


	float time = 0f;

	int Lerp(int start, int end, float time)
	{
		return (int)(start * (1-time) + end * time);
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update();

		int angle  = Lerp(0, 360, time);

		m_weaponHolder.GetWeapon().StartFiring(RotateToTarget(angle), 0, m_firingDescs);

		time += Time.deltaTime * 0.1f;
		time -= (int)time;
	}

}
