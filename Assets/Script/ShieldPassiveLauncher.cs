using UnityEngine;
using System.Collections;

public class ShieldPassiveLauncher : Weapon {

	[SerializeField]
	GameObject m_prefChargingEffect;

	Bullet	m_bullet;
	float	m_accSp;

	new void Start()
	{
		base.Start();

		StartCoroutine(EffectShield());
	}

	override public void StartFiring(float targetAngle)
	{		
		if (canConsumeSP() == true )
		{
			if (m_firing == false)
			{
				if (m_bullet == null)
				{
					m_bullet = CreateBullet(m_firingDescs[0], m_gunPoint.transform.position) as Bullet;
				}

				this.audio.Play();
			}
			else
			{
				m_accSp += SP * Time.deltaTime * coolDownTime();
				if (m_accSp >= 1)
				{
					m_creature.m_creatureProperty.SP -= (int)m_accSp;
					m_accSp -= (int)m_accSp;
				}
			}
		}
		
		m_firing = true;
	}
	
	override public void StopFiring()
	{
		base.StopFiring();
		this.audio.Stop();		
	}

	IEnumerator EffectShield()
	{
		DamageNumberSprite sprite = m_creature.DamageText("", Color.white, DamageNumberSprite.MovementType.FloatingUpAlways);		
		Vector3 scale = sprite.gameObject.transform.localScale;
		scale *= 0.5f;
		sprite.gameObject.transform.localScale = scale;

		GameObject obj = Instantiate (m_prefChargingEffect, Vector3.zero, transform.rotation) as GameObject;		
		obj.transform.parent = m_creature.transform;
		obj.transform.localPosition = m_prefChargingEffect.transform.localPosition;
		obj.transform.localScale = m_prefChargingEffect.transform.localScale;
		obj.transform.localRotation = m_prefChargingEffect.transform.localRotation;

		while(true)
		{
			bool shield = m_creature.m_creatureProperty.Shield > 0;
			if (shield == true)
			{
				sprite.Text = "Shield " + m_creature.m_creatureProperty.Shield;
			}
			
			sprite.gameObject.SetActive(shield);
			obj.SetActive(shield);
			
			yield return null;
		}
	}

	override public void LevelUp()
	{
		m_level = Mathf.Min(m_level+1, 1);

		m_creature.m_creatureProperty.Shield += 10;
	}

}
