using UnityEngine;
using System.Collections;

public class MineBullet : GrenadeBullet {


	float			m_elapsed = 0f;

	[SerializeField]
	float			m_bombTime = 3f;

	// Use this for initialization
	void Start () {

	}

	override public void Init(Creature ownerCreature, Vector3 gunPoint, int damage, Weapon.FiringDesc targetAngle, Weapon onHitWeapon)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle, onHitWeapon);
	
		m_elapsed = Time.time+m_bombTime;

	}

	protected override void createParabola(float targetAngle)
	{
		base.createParabola(Random.Range(0f, 360f)+targetAngle);
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
