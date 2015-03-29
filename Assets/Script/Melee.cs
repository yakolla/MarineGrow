using UnityEngine;
using System.Collections;

public class Melee : Weapon {
	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(targetAngle, startPos);
		Vector3 scale = bullet.transform.localScale;
		scale.x = m_attackRange;
		bullet.transform.localScale = scale;

		return bullet;
	}
}
