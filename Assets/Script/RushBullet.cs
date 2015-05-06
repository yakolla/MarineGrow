using UnityEngine;
using System.Collections;

public class RushBullet : Bullet {
	
	// Use this for initialization
	void Start () 
	{		

	}

	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);
		
		transform.parent = m_gunPoint.transform;
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
