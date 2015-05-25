using UnityEngine;
using System.Collections;

public class RushBullet : Bullet {
	
	// Use this for initialization
	void Start () 
	{		

	}

	override public void Init(Creature ownerCreature, Vector3 gunPoint, int damage, Vector2 targetAngle, Weapon onHitWeapon)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle, onHitWeapon);
		
		transform.parent = ownerCreature.WeaponHolder.transform;
		transform.localPosition = Vector3.zero;
	}

	void OnTriggerEnter(Collider other) {

		
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}

	}
}
