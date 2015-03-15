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
			m_bulletSpeed += m_level;
		}
	}
	
	override public Bullet CreateBullet(Vector2 targetAngle, Vector3 startPos)
	{
		Bullet bullet = base.CreateBullet(new Vector2(transform.rotation.eulerAngles.y, 0), m_gunPoint.transform.position);
		bullet.GetComponent<GunBullet>().BulletSpeed = m_bulletSpeed;
		return bullet;
	}
}