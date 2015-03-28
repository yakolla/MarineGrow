using UnityEngine;
using System.Collections;

public class FallenSuicideBombing : Weapon {

	bool m_destroy = false;

	override public void StartFiring(Vector2 targetAngle)
	{
		m_firing = true;
	}

	IEnumerator EffectEgg()
	{
		yield return new WaitForSeconds (3f);		
		m_creature.Death();
		
	}

	void Update()
	{
		if (m_destroy == true)
		{
			return;
		}

		if (m_creature.transform.position.y <= 0f)
		{
			CreateBullet(Vector2.zero, transform.position);
			StartCoroutine(EffectEgg());

			m_destroy = true;
		}
	}

}
