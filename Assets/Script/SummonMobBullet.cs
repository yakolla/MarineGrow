using UnityEngine;
using System.Collections;

public class SummonMobBullet : GrenadeBullet {

	EffectTargetingPoint	m_effectTargetingPoint = null;
	Creature		m_spawnedMob = null;
	// Use this for initialization

	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		m_effectTargetingPoint = new EffectTargetingPoint();
		base.Init(ownerCreature, gunPoint, damage, targetAngle);

		Mob ownerMob = (Mob)m_ownerCreature;
		if (ownerMob != null)
		{
			m_spawnedMob = m_ownerCreature.Spawn.SpawnMob(ownerMob.RefMob.dropEggMob.refMob, gameObject.transform.position, false, false);
		}

	}

	protected override void createParabola(Vector2 targetAngle)
	{
		//m_parabola = new Parabola(gameObject, Random.Range(1f, m_speed), -targetAngle.x * Mathf.Deg2Rad, 1.3f, m_bouncing);
		m_parabola = new Parabola(gameObject, m_speed, -targetAngle.x * Mathf.Deg2Rad, Random.Range(1f, 1.4f), m_bouncing);
		m_effectTargetingPoint.Init(m_parabola.DestPosition);
	}

	// Update is called once per frame
	new void Update () {
		base.Update();

		if (m_spawnedMob != null)
			m_spawnedMob.transform.position = m_parabola.Position;
	}	

	protected override void bomb()
	{
		base.bomb();
		m_effectTargetingPoint.Death();
	}
}