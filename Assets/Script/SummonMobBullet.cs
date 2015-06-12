using UnityEngine;
using System.Collections;

public class SummonMobBullet : GrenadeBullet {

	EffectTargetingPoint	m_effectTargetingPoint = null;
	Creature		m_spawnedMob = null;
	// Use this for initialization

	override public void Init(Creature ownerCreature, Vector3 gunPoint, int damage, Weapon.FiringDesc targetAngle, Weapon onHitWeapon)
	{
		m_effectTargetingPoint = new EffectTargetingPoint();
		base.Init(ownerCreature, gunPoint, damage, targetAngle, onHitWeapon);

		m_spawnedMob = Const.GetSpawn().SpawnMob(m_ownerCreature.RefMob.dropEggMob.refMob, gameObject.transform.position, false, false);
		m_spawnedMob.CreatureType = ownerCreature.CreatureType;
		m_spawnedMob.SetTarget(null);
	}

	protected override void createParabola(float targetAngle)
	{
		m_parabola = new Parabola(gameObject, m_speed, -(transform.rotation.eulerAngles.y+targetAngle) * Mathf.Deg2Rad, Random.Range(1f, 1.4f), m_bouncing);
		m_effectTargetingPoint.Init(m_parabola.DestPosition);
	}

	// Update is called once per frame
	new void Update () {
		base.Update();

		if (m_spawnedMob != null)
			m_spawnedMob.transform.position = m_parabola.Position;
	}	

	protected override void bomb(float bombRange, GameObject prefBombEffect)
	{
		base.bomb(bombRange, prefBombEffect);
		m_effectTargetingPoint.Death();
	}
}
