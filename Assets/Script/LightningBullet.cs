/*
	This script is placed in public domain. The author takes no responsibility for any possible harm.
	Contributed by Jonathan Czeck
*/
using UnityEngine;
using System.Collections;

public class LightningBullet : Bullet
{
	public int zigs = 100;
	public float speed = 1f;
	public float scale = 1f;
	public float length = 10f;
	public float	m_damageOnTime = 1f;
	public Color	m_color = Color.white;
	float			m_lastDamageTime = 0f;
	public int	m_maxChaining = 5;

	Perlin noise;
	float oneOverZigs;
	
	private Particle[] particles;

	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, Vector2 targetAngle)
	{
		Vector3 scale = transform.localScale;

		base.Init(ownerCreature, gunPoint, damage, targetAngle);
		transform.parent = m_gunPoint.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(new Vector3(0, targetAngle.x, 0));
		transform.localScale = scale;
	}
	
	void Start()
	{
		oneOverZigs = 1f / (float)zigs;
		particleEmitter.emit = false;

		particleEmitter.Emit(zigs);
		particles = particleEmitter.particles;
		noise = new Perlin();

		for(int i = 0; i < particles.Length; ++i)
		{
			particles[i].position = Vector3.zero;
			particles[i].color = m_color;
			particles[i].energy = 1f;
		}
		particleEmitter.particles = particles;
	}
	
	void Update ()
	{

		bool mobHitted = false;

		Creature[] targets = null;
		int hittedTargetCount = 0;
		if (m_maxChaining > 0)
		{
			targets = new Creature[m_maxChaining];
			RaycastHit hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			if (Physics.Raycast(transform.position, fwd, out hit, length))
			{
				Creature creature = hit.transform.gameObject.GetComponent<Creature>();
				if (creature && Creature.IsEnemy(creature, m_ownerCreature))
				{				
					targets[0] = creature;
					mobHitted = true;
					hittedTargetCount = 1;
					
					for(int i = 1; i < targets.Length; ++i)
					{
						GameObject chaningTargetObj = targets[i-1].SearchTarget(m_ownerCreature.GetAutoTargetTags(), targets, 3f);
						if (chaningTargetObj == null)
							break;
						
						targets[i] = chaningTargetObj.GetComponent<Creature>();
						hittedTargetCount++;
					}
					
					int perParticles = particles.Length/hittedTargetCount;
					createChanningParticle(transform.position, targets[0].transform.position, 0, perParticles);
					for(int i = 0; i < hittedTargetCount-1; ++i)
					{
						createChanningParticle(targets[i].transform.position, targets[i+1].transform.position, perParticles*(i+1), perParticles*(i+1)+perParticles);
					}
					
					particleEmitter.particles = particles;
				}
			}
		}
		else
		{
			RaycastHit[] hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			hit = Physics.RaycastAll(transform.position, fwd, length);
			if (hit.Length > 0)
			{
				targets = new Creature[hit.Length];
				for(int i = 0; i < hit.Length; ++i)
				{
					Creature creature = hit[i].transform.gameObject.GetComponent<Creature>();
					if (creature && Creature.IsEnemy(creature, m_ownerCreature))
					{				
						targets[hittedTargetCount] = creature;
						hittedTargetCount += 1;
					}
				}

			}
		}

		if (targets != null)
		{
			if (m_lastDamageTime+(m_damageOnTime*m_ownerCreature.m_creatureProperty.AttackCoolTime)<Time.time)
			{
				for(int i = 0; i < hittedTargetCount; ++i)
				{
					targets[i].TakeDamage(m_ownerCreature, new DamageDesc(m_damage, DamageDesc.Type.Lightining, m_damageBuffType, PrefDamageEffect));
				}
				
				m_lastDamageTime = Time.time;
			}
		}

		if (mobHitted == false)
		{
			Vector3 targetPos = new Vector3();
			targetPos.x = Mathf.Cos(transform.rotation.eulerAngles.y*Mathf.Deg2Rad)*length;
			targetPos.z = Mathf.Sin(transform.rotation.eulerAngles.y*Mathf.Deg2Rad)*-length;
			targetPos.x += transform.position.x;
			targetPos.z += transform.position.z;

			createChanningParticle(transform.position, targetPos, 0, particles.Length);
			
			particleEmitter.particles = particles;
		}
	}	

	void createChanningParticle(Vector3 start, Vector3 dest, int particleStartIndex, int particleFinishIndex)
	{
		float timex = Time.time * speed * 0.1365143f;
		float timey = Time.time * speed * 1.21688f;
		float timez = Time.time * speed * 2.5564f;


		int length = particleFinishIndex-particleStartIndex;
		for (int i=0; i < length; i++)
		{
			float lerpProgress = oneOverZigs * (float)i * (particles.Length/length);
			Vector3 position = Vector3.Lerp(start, dest, lerpProgress);
			Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
			                             noise.Noise(timey + position.x, timey + position.y, timey + position.z),
			                             noise.Noise(timez + position.x, timez + position.y, timez + position.z));
			position += offset * scale * lerpProgress;
			
			particles[particleStartIndex+i].position = position;
			particles[particleStartIndex+i].color = m_color;
			particles[particleStartIndex+i].energy = 1f;
		}
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(this.gameObject);
	}
}