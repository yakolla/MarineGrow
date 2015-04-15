using UnityEngine;
using System.Collections;

public class Melee : Weapon {

	override protected Weapon.FiringDesc DefaultFiringDesc()
	{
		Weapon.FiringDesc desc = new Weapon.FiringDesc();
		desc.angle = 0;
		desc.delayTime = 0.3f;
		
		return desc;
	}

	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(targetAngle, startPos);
		Vector3 scale = bullet.transform.localScale;
		scale.x = m_attackRange;
		bullet.transform.localScale = scale;

		return bullet;
	}
}
