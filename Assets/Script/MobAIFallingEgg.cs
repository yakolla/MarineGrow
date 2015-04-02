using UnityEngine;
using System.Collections;

public class MobAIFallingEgg : MobAI {

	Parabola 	m_parabola;
	
	bool		m_ingDeathEffect;

	Vector3		m_oriPos;
	Vector3		m_targetPos;

	float		m_elapsed = 0f;
	float		m_speed = 0.8f;

	GameObject	m_prefEffectTargetPoint = Resources.Load<GameObject>("Pref/ef_targeting");

	GameObject	m_effectTargetPoint;

	override public void	Init(Creature mob)
	{
		base.Init(mob);
		m_targetPos = mob.transform.position;
		m_targetPos.y = 0;

		m_effectTargetPoint = GameObject.Instantiate (m_prefEffectTargetPoint, m_targetPos, m_prefEffectTargetPoint.transform.localRotation) as GameObject;
		ParticleSystem particle = m_effectTargetPoint.GetComponent<ParticleSystem>();
		particle.Play();

		Vector3 pos = mob.gameObject.transform.position;
		pos.y = 40f;
		mob.gameObject.transform.position = pos;
		m_oriPos = pos;
	}

	// Update is called once per frame
	override public void Update () {

		m_elapsed += Time.deltaTime*m_speed;
		m_mob.transform.position = Vector3.Lerp(m_oriPos, m_targetPos, Mathf.Min(1f, m_elapsed));

		if (m_elapsed > 1f)
		{
			if (m_ingDeathEffect == false)
			{
				m_ingDeathEffect = true;
				GameObject.DestroyObject(m_effectTargetPoint);

			}
		}

		if (m_mob.AutoAttack() == false)
		{
			if (m_target)
			{
				m_mob.RotateToTarget(m_target.transform.position);
			}
		}
	}

}
