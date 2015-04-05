using UnityEngine;
using System.Collections;

public class SummonMobBullet : GrenadeBullet {

	EffectTargetingPoint	m_effectTargetingPoint = new EffectTargetingPoint();
	Creature		m_spawnedMob = null;
	// Use this for initialization
	void Start () {


	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);

		Mob ownerMob = (Mob)m_ownerCreature;
		if (ownerMob != null)
		{
			m_spawnedMob = m_ownerCreature.Spawn.SpawnMob(ownerMob.RefMob.dropEggMob.refMob, gameObject.transform.position, false, false);
		}

		m_effectTargetingPoint.Init(gameObject.transform.position+gameObject.transform.forward*m_speed);

	}

	protected override void createParabola(Vector2 targetAngle)
	{
		m_parabola = new Parabola(gameObject, Random.Range(1f, m_speed), 15f, -targetAngle.x * Mathf.Deg2Rad, 1.3f, m_bouncing);
	}

	// Update is called once per frame
	protected override void Update () {
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
