/*
	This script is placed in public domain. The author takes no responsibility for any possible harm.
	Contributed by Jonathan Czeck
*/
using UnityEngine;
using System.Collections;

public class LightningBoltBullet : Bullet
{
	Vector3 targetPos;
	public int zigs = 100;
	public float speed = 1f;
	public float scale = 1f;
	public float length = 10f;
	public Light startLight;
	public GameObject endLight;
	public float	m_coolTime = 0.3f;
	float			m_lastDamageTime = 0f;
	Perlin noise;
	float oneOverZigs;
	
	private Particle[] particles;

	override public void Init(Creature ownerCreature, GameObject gunPoint, float damage, float chargingTime, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, chargingTime, targetAngle);
		this.transform.parent = m_gunPoint.transform;
	}
	
	void Start()
	{
		oneOverZigs = 1f / (float)zigs;
		particleEmitter.emit = false;

		particleEmitter.Emit(zigs);
		particles = particleEmitter.particles;

		targetPos = transform.position;
	}
	
	void Update ()
	{

		bool mobHitted = false;

		Creature target = null;
		if (m_ownerCreature.m_targeting)
		{
			targetPos = m_ownerCreature.m_targeting.transform.position;
			target = (Creature)m_ownerCreature.m_targeting.GetComponent<Creature>();
			mobHitted = true;
		}
		else
		{
			RaycastHit hit;
			Vector3 fwd = transform.TransformDirection(Vector3.right);
			if (Physics.Raycast(transform.position, fwd, out hit, length))
			{
				if (hit.transform.tag.CompareTo(m_targetTagName) == 0)
				{
					targetPos = hit.transform.position;
					target = (Creature)hit.transform.gameObject.GetComponent<Creature>();
					mobHitted = true;
				}
			}
		}

		if (mobHitted == true)
		{
			if (m_lastDamageTime+m_coolTime<Time.time)
			{
				target.TakeDamage(m_ownerCreature, new DamageDesc(m_ownerCreature.m_creatureProperty.PhysicalAttackDamage*m_coolTime, DamageDesc.Type.Lightinig, endLight));
				m_lastDamageTime = Time.time;
			}
		}
		else
		{
			targetPos.x = Mathf.Cos(transform.rotation.eulerAngles.y*Mathf.Deg2Rad)*length;
			targetPos.z = Mathf.Sin(transform.rotation.eulerAngles.y*Mathf.Deg2Rad)*-length;
			targetPos.x += transform.position.x;
			targetPos.z += transform.position.z;
		}

		if (noise == null)
			noise = new Perlin();
			
		float timex = Time.time * speed * 0.1365143f;
		float timey = Time.time * speed * 1.21688f;
		float timez = Time.time * speed * 2.5564f;
		
		for (int i=0; i < particles.Length; i++)
		{
			Vector3 position = Vector3.Lerp(transform.position, targetPos, oneOverZigs * (float)i);
			Vector3 offset = new Vector3(noise.Noise(timex + position.x, timex + position.y, timex + position.z),
										noise.Noise(timey + position.x, timey + position.y, timey + position.z),
										noise.Noise(timez + position.x, timez + position.y, timez + position.z));
			position += (offset * scale * ((float)i * oneOverZigs));
			
			particles[i].position = position;
			particles[i].color = Color.white;
			particles[i].energy = 1f;
		}
		
		particleEmitter.particles = particles;
		
		if (particleEmitter.particleCount >= 2)
		{
			if (startLight)
				startLight.transform.position = particles[0].position;
			//if (endLight)
			//	endLight.transform.position = particles[particles.Length - 1].position;
		}
	}	
	
	override public void StopFiring()
	{
		base.StopFiring();
		DestroyObject(this.gameObject);
	}
}