using UnityEngine;
using System.Collections;

public class Follower : Creature {


	Creature	m_owner;
	MobAI		m_ai;
	int			m_refMobId;

	// Update is called once per frame
	new void Update () {

		base.Update();

		if (AutoAttack() == false)
		{
		}

		if (m_owner != null)
			m_navAgent.SetDestination(m_owner.transform.position);
		
	}

	override public bool AutoAttack() {
		
		
		if (HasCrowdControl() == false)
		{
			if (Targetting == null)
			{
				SetTarget(SearchTarget(GetMyEnemyType(), null, 50f));
			}
			
			if (Targetting != null)
			{
				if (true == inAttackRange(Targetting, 0f))
				{
					m_weaponHolder.StartFiring(RotateToTarget(Targetting.transform.position));
					return true;
				}
			}
		}
		
		SetTarget(null);
		m_weaponHolder.StopFiring();
		return false;
	}

	IEnumerator DecHpEffect()
	{
		while(m_creatureProperty.HP > 0)
		{
			yield return new WaitForSeconds(1f);
			m_creatureProperty.HP -= 1;
		}

		Death();
	}

	public void Init(Creature owner, RefMob refMob, int level)
	{
		base.Init(refMob, level);

		m_owner = owner;
		CreatureType = m_owner.CreatureType;

		if (m_creatureProperty.MoveSpeed == 0f)
		{
			EnableNavMeshObstacleAvoidance(false);
		}

		switch(refMob.mobAI)
		{
		case MobAIType.Normal:
			m_ai = new MobAINormal();
			break;
		case MobAIType.Rotation:
			m_ai = new MobAIRotation();
			break;
		case MobAIType.Revolution:
			m_ai = new MobAIRevolution();
			break;
		case MobAIType.ItemShuttle:
			m_ai = new MobAIItemShuttle();
			break;
		}
		
		m_ai.Init(this);

		StartCoroutine(DecHpEffect());
	
	}

	public int FollowerID
	{
		get{return m_refMobId;}
	}

	void LevelUp()
	{
		ApplyBuff(null, DamageDesc.BuffType.LevelUp, 10f, null);
		WeaponHolder.LevelUp();
	}


}
