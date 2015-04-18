using UnityEngine;
using System.Collections;

public class MineBullet : Bullet {



	[SerializeField]
	GameObject		m_prefBombEffect = null;

	[SerializeField]
	float			m_bombRange = 5f;

	float			m_elapsed = 0f;

	[SerializeField]
	float			m_bombTime = 3f;

	// Use this for initialization
	void Start () {

	}

	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);
	
		m_elapsed = Time.time+m_bombTime;

	}

	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		if (m_elapsed < Time.time)
		{
			bomb(m_bombRange, m_prefBombEffect);
		}
	}

}
