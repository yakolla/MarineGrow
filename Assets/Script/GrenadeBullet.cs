using UnityEngine;
using System.Collections;

public class GrenadeBullet : Bullet {

	bool m_isDestroying = false;

	[SerializeField]
	GameObject		m_prefBombEffect = null;

	[SerializeField]
	float			m_bombRange = 5f;

	[SerializeField]
	protected float			m_speed = 7f;

	[SerializeField]
	protected int				m_bouncing = 1;

	protected Parabola	m_parabola;
	// Use this for initialization
	void Start () {


	}
	override public void Init(Creature ownerCreature, GameObject gunPoint, int damage, Vector2 targetAngle)
	{
		base.Init(ownerCreature, gunPoint, damage, targetAngle);
		createParabola(targetAngle);
	}

	protected virtual void createParabola(Vector2 targetAngle)
	{
		m_parabola = new Parabola(gameObject, Random.Range(1f, m_speed), 10f, -targetAngle.x * Mathf.Deg2Rad, 45f * Mathf.Deg2Rad, m_bouncing);
	}

	// Update is called once per frame
	protected virtual void Update () {
		if (m_isDestroying == true)
			return;

		if (m_parabola.Update() == false)
		{
			bomb();
		}

	}
	
	IEnumerator destoryObject(GameObject bombEffect)
	{
		yield return new WaitForSeconds (bombEffect.particleSystem.duration);
		DestroyObject(this.gameObject);
		DestroyObject(bombEffect);
	}

	protected virtual void bomb()
	{
		m_isDestroying = true;

		string[] tags = m_ownerCreature.GetAutoTargetTags();
		foreach(string tag in tags)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
			Vector3 pos = transform.position;
			//pos.y = 0;
			foreach(GameObject target in targets)
			{
				float dist = Vector3.Distance(pos, target.transform.position);
				if (dist < m_bombRange/2)
				{
					Creature creature = target.GetComponent<Creature>();
					GiveDamage(creature);
				}
			}
		}

		Vector3 bombPos = transform.position;
		bombPos.y = m_prefBombEffect.transform.position.y;

		GameObject bombEffect = (GameObject)Instantiate(m_prefBombEffect, bombPos, m_prefBombEffect.transform.rotation);
		bombEffect.particleSystem.startSize = m_bombRange*2;
		this.audio.Play();
		StartCoroutine(destoryObject(bombEffect));
	}
}
