using UnityEngine;
using System.Collections;

public class NuclearBullet : GrenadeBullet {


	protected override void createParabola(float targetAngle)
	{
		m_parabola = new Parabola(gameObject, m_speed, 0, 90 * Mathf.Deg2Rad, m_bouncing);
	}
}
