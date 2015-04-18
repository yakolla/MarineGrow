using UnityEngine;
using System.Collections;

public class SpiderWebBullet : Bullet {


	[SerializeField]
	GameObject		m_prefBombEffect = null;
	
	[SerializeField]
	float			m_bombRange = 5f;

	[SerializeField]
	float			m_speed = 7f;
	
	[SerializeField]
	int				m_bouncing = 1;

	Parabola	m_parabola;
	// Use this for initialization
	void Start () {
		
		
	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);
		

		Vector3 scale = Vector3.one;
		scale.x += m_bombRange/2f;
		scale.y += m_bombRange/2f;
		m_parabola = new Parabola(gameObject, m_speed, -targetAngle.x * Mathf.Deg2Rad, Random.Range(55f,85f) * Mathf.Deg2Rad, m_bouncing);

		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_isDestroying == true)
			return;

		if (m_parabola.Update() == false)
		{
			bomb(m_bombRange, m_prefBombEffect);
		}


	}
	


	void OnTriggerEnter(Collider other) {
		Creature creature = other.gameObject.GetComponent<Creature>();
		if (creature && Creature.IsEnemy(creature, m_ownerCreature))
		{
			GiveDamage(creature);
		}
	}


}
