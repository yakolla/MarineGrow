using UnityEngine;
using System.Collections;

public class PumpLauncher : Weapon {

	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		return base.CreateBullet(targetAngle, m_creature.transform.position);
	}

	override public void StartFiring(Vector2 targetAngle)
	{
	}

	override public void StopFiring()
	{	
	}

	void Update()
	{
		if (isCoolTime())
		{
			base.StartFiring(new Vector2(0, 0));
		}
	}
}
