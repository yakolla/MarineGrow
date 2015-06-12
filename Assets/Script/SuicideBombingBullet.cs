using UnityEngine;
using System.Collections;

public class SuicideBombingBullet : Bullet {


	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;

	// Use this for initialization

	void Update()
	{
		if (m_isDestroying == true)
			return;

		bomb(m_bombRange, m_prefBombEffect);
	}
}
