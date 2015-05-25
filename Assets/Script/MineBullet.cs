using UnityEngine;
using System.Collections;

public class MineBullet : GrenadeBullet {


	float			m_elapsed = 0f;

	[SerializeField]
	float			m_bombTime = 3f;

	// Use this for initialization
	void Start () {

	}

	override public void Init(Creature ownerCreature, Vector3 gunPoint, int damage, Vector2 targetAngle, Weapon onHitWeapon)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle, onHitWeapon);
	
		m_elapsed = Time.time+m_bombTime;

	}

	// Update is called once per frame
	new void Update () {
		if (m_isDestroying == true)
			return;
		if (m_parabola.Update() == false)
		{
			if (m_elapsed < Time.time)
			{
				bomb(m_bombRange, m_prefBombEffect);
			}
		}

	}

}
